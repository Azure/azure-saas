using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Saas.Permissions.Service.Swagger;

public class SwagCustomHeaderFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Parameters ??= new List<OpenApiParameter>();

        operation.Parameters.Add(new OpenApiParameter()
        {
            Name = "x-api-key",
            Description = "API key",
            In = ParameterLocation.Header,
            Required = false,
            Example = new OpenApiString("<some secret string>"),
            Schema = new OpenApiSchema
            {
                Type = "String"
            },
        });
    }
}
