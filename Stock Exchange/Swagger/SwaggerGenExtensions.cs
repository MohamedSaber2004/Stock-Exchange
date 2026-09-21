using Asp.Versioning.ApiExplorer;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using System.IO;

namespace Stock_Exchange.Swagger;

public static class SwaggerGenExtensions
{
    public static SwaggerGenOptions AddAcceptLanguageHeader(this SwaggerGenOptions options)
    {
        options.OperationFilter<AcceptLanguageHeaderOperationFilter>();
        return options;
    }

    public static SwaggerGenOptions AddJwtBearerSecurity(this SwaggerGenOptions options)
    {
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Enter JWT Bearer token."
        });

        options.OperationFilter<AuthorizeCheckOperationFilter>();

        return options;
    }

    public static SwaggerGenOptions IncludeApiXmlComments(this SwaggerGenOptions options)
    {
        var executingAssembly = Assembly.GetExecutingAssembly();
        var apiXmlPath = Path.Combine(AppContext.BaseDirectory, $"{executingAssembly.GetName().Name}.xml");

        if (File.Exists(apiXmlPath))
        {
            options.IncludeXmlComments(apiXmlPath, includeControllerXmlComments: true);
        }

        var otherXmlFiles = Directory.GetFiles(AppContext.BaseDirectory, "*.xml")
            .Where(f => !string.Equals(f, apiXmlPath, StringComparison.OrdinalIgnoreCase) &&
                        (Path.GetFileName(f).StartsWith("Stock", StringComparison.OrdinalIgnoreCase) ||
                         Path.GetFileName(f).Contains("Exchange", StringComparison.OrdinalIgnoreCase)))
            .ToList();

        foreach (var xmlPath in otherXmlFiles)
        {
            options.IncludeXmlComments(xmlPath);
        }

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