using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CheckadorAPI.Swagger;

/// <summary>
/// Filtro que agrega automáticamente respuestas comunes a todos los endpoints
/// </summary>
public class SwaggerDefaultResponsesFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        // Respuesta 500 - Error interno del servidor
        if (!operation.Responses.ContainsKey("500"))
        {
            operation.Responses.Add("500", new OpenApiResponse
            {
                Description = "Error interno del servidor"
            });
        }

        // Respuesta 400 - Bad Request (para POST/PUT)
        var httpMethod = context.ApiDescription.HttpMethod?.ToUpper();
        if ((httpMethod == "POST" || httpMethod == "PUT" || httpMethod == "PATCH") && 
            !operation.Responses.ContainsKey("400"))
        {
            operation.Responses.Add("400", new OpenApiResponse
            {
                Description = "Solicitud inválida - Datos incorrectos o faltantes"
            });
        }
    }
}
