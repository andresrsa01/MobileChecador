using System.Reflection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CheckadorAPI.Swagger;

public static class SwaggerConfigurationExtensions
{
    public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            // Información general del API
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "Checador API",
                Description = @"API REST completa para el sistema de checador de asistencias con geolocalización.
                
**Características principales:**
- Autenticación JWT con roles (Administrador/Empleado)
- Registro de asistencias con validación de geolocalización
- Gestión de usuarios y configuraciones de geofence
- Auditoría completa de accesos",
                Contact = new OpenApiContact
                {
                    Name = "Soporte Técnico - Checador API",
                    Email = "soporte@checador.com",
                    Url = new Uri("https://github.com/andresrsa01/MobileChecador")
                },
                License = new OpenApiLicense
                {
                    Name = "MIT License",
                    Url = new Uri("https://opensource.org/licenses/MIT")
                }
            });

            // Configuración de seguridad JWT
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = @"Autenticación JWT usando el esquema Bearer.
                
**Instrucciones:**
1. Ejecuta el endpoint POST /api/Auth/login con tus credenciales
2. Copia el token recibido en la respuesta
3. Haz clic en el botón 'Authorize' arriba
4. Ingresa: Bearer {tu_token}
5. Haz clic en 'Authorize' y luego en 'Close'

**Ejemplo:** `Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...`"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });

            // Incluir comentarios XML si existen
            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
            if (File.Exists(xmlPath))
            {
                options.IncludeXmlComments(xmlPath);
            }

            // Ordenar acciones por método HTTP
            options.OrderActionsBy(apiDesc => $"{apiDesc.ActionDescriptor.RouteValues["controller"]}_{apiDesc.HttpMethod}");

            // Filtros personalizados
            options.OperationFilter<SwaggerDefaultResponsesFilter>();
            options.OperationFilter<SwaggerAuthenticationResponseFilter>();
            options.DocumentFilter<SwaggerTagDescriptionsFilter>();

            // Personalizar nombres de esquemas usando el nombre completo para evitar conflictos
            options.CustomSchemaIds(type => type.FullName?.Replace("+", "."));
        });

        return services;
    }

    public static IApplicationBuilder UseSwaggerConfiguration(this IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseSwagger(c =>
            {
                c.RouteTemplate = "swagger/{documentName}/swagger.json";
            });

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Checador API v1");
                c.DocumentTitle = "Checador API - Documentación";
                c.RoutePrefix = "swagger";

                // Mejoras visuales
                c.DefaultModelsExpandDepth(2);
                c.DefaultModelRendering(Swashbuckle.AspNetCore.SwaggerUI.ModelRendering.Example);
                c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
                c.DisplayRequestDuration();
                c.EnableDeepLinking();
                c.EnableFilter();
                c.ShowExtensions();
                c.EnableValidator();

                // CSS personalizado para mejorar la apariencia
                c.InjectStylesheet("/swagger-custom.css");
            });
        }

        return app;
    }
}
