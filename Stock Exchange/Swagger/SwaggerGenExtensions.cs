using Asp.Versioning.ApiExplorer;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace Stock_Exchange.Swagger;

public static class SwaggerGenExtensions
{
    public static SwaggerGenOptions AddAcceptLanguageHeader(this SwaggerGenOptions options)
    {
        options.OperationFilter<AcceptLanguageHeaderOperationFilter>();
        return options;
    }

    public static SwaggerGenOptions IncludeApiXmlComments(this SwaggerGenOptions options)
    {
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

        if (File.Exists(xmlPath))
            options.IncludeXmlComments(xmlPath);

        return options;
    }

    public static void AddVersionedSwaggerDocs(this SwaggerGenOptions options, IApiVersionDescriptionProvider provider)
    {
        foreach (var description in provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
        }
    }

    private static OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
    {
        var text = description.IsDeprecated ? "This API version has been deprecated." : "The API is supported and stable.";

        return new OpenApiInfo
        {
            Title = "Stock Exchange API",
            Version = description.ApiVersion.ToString(),
            Description = $"Stock Exchange API {description.GroupName}. {text}"
        };
    }
}
