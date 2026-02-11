using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CheckadorAPI.Swagger;

/// <summary>
/// Filtro que agrega descripciones detalladas a los tags (agrupaciones de controladores)
/// </summary>
public class SwaggerTagDescriptionsFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        swaggerDoc.Tags = new List<OpenApiTag>
        {
            new OpenApiTag
            {
                Name = "Auth",
                Description = @"**Autenticación y Autorización**
                
Endpoints para gestionar el acceso al sistema:
- Login con credenciales (usuario/contraseña)
- Generación de tokens JWT
- Validación de permisos"
            },
            new OpenApiTag
            {
                Name = "Users",
                Description = @"**Gestión de Usuarios**
                
Administración completa de usuarios del sistema:
- Crear, leer, actualizar y eliminar usuarios
- Gestión de roles (Administrador/Empleado)
- Consulta de historial de accesos
- Control de usuarios activos/inactivos

?? **Nota:** La mayoría de estos endpoints requieren rol de Administrador"
            },
            new OpenApiTag
            {
                Name = "Attendance",
                Description = @"**Registro de Asistencias**
                
Control y seguimiento de asistencias:
- Registro de entrada/salida con validación de geolocalización
- Consulta de historial de asistencias
- Filtros por usuario y rango de fechas
- Validación automática contra configuración de geofence"
            },
            new OpenApiTag
            {
                Name = "Geofence",
                Description = @"**Configuración de Geofence**
                
Gestión de zonas geográficas permitidas:
- Configurar ubicación y radio permitido
- Administrar geofences por usuario
- Validación de proximidad
- Actualización de parámetros de ubicación"
            }
        };
    }
}
