using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CheckadorAPI.Swagger;

/// <summary>
/// Filtro que agrega respuestas de autenticación/autorización a endpoints protegidos
/// </summary>
public class SwaggerAuthenticationResponseFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        // Verificar si el endpoint requiere autenticación
        var hasAuthorize = context.MethodInfo.DeclaringType?.GetCustomAttributes(true)
            .OfType<AuthorizeAttribute>().Any() ?? false;

        if (!hasAuthorize)
        {
            hasAuthorize = context.MethodInfo.GetCustomAttributes(true)
                .OfType<AuthorizeAttribute>().Any();
        }

        // Verificar si tiene AllowAnonymous
        var allowAnonymous = context.MethodInfo.DeclaringType?.GetCustomAttributes(true)
            .OfType<AllowAnonymousAttribute>().Any() ?? false;

        if (!allowAnonymous)
        {
            allowAnonymous = context.MethodInfo.GetCustomAttributes(true)
                .OfType<AllowAnonymousAttribute>().Any();
        }

        // Si requiere autenticación y no tiene AllowAnonymous
        if (hasAuthorize && !allowAnonymous)
        {
            // Agregar respuesta 401
            if (!operation.Responses.ContainsKey("401"))
            {
                operation.Responses.Add("401", new OpenApiResponse
                {
                    Description = "No autorizado - Token JWT faltante o inválido"
                });
            }

            // Verificar si tiene requisito de rol
            var authorizeAttributes = context.MethodInfo.GetCustomAttributes(true)
                .OfType<AuthorizeAttribute>()
                .Union(context.MethodInfo.DeclaringType?.GetCustomAttributes(true)
                    .OfType<AuthorizeAttribute>() ?? Enumerable.Empty<AuthorizeAttribute>());

            var hasRoleRequirement = authorizeAttributes.Any(a => !string.IsNullOrEmpty(a.Roles));

            if (hasRoleRequirement && !operation.Responses.ContainsKey("403"))
            {
                operation.Responses.Add("403", new OpenApiResponse
                {
                    Description = "Prohibido - No tienes permisos suficientes para acceder a este recurso"
                });
            }

            // Agregar badge de seguridad
            operation.Summary = $"?? {operation.Summary ?? ""}";
        }
        else
        {
            // Agregar badge de público
            operation.Summary = $"?? {operation.Summary ?? ""}";
        }
    }
}
