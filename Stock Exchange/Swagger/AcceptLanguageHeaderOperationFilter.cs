using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Stock_Exchange.Swagger;

public class AcceptLanguageHeaderOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (operation.Parameters == null)
            operation.Parameters = new List<OpenApiParameter>();

        if (operation.Parameters.All(p => p.Name != "Accept-Language"))
        {
            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "Accept-Language",
                In = ParameterLocation.Header,
                Required = false,
                Description = "Preferred language for the response. Supported values: `ar`, `en`.",
                Schema = new OpenApiSchema
                {
                    Type = "string",
                    Default = new OpenApiString("ar"),
                    Enum = new List<IOpenApiAny>
                    {
                        new OpenApiString("ar"),
                        new OpenApiString("en")
                    }
                }
            });
        }
    }
}
