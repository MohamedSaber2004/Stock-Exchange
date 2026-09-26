using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Stock_Exchange.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Stock_Exchange.Swagger;

public class AuthorizeCheckOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var hasAllowAnonymous = context.MethodInfo.GetCustomAttributes(true).OfType<AllowAnonymousAttribute>().Any()
            || (context.MethodInfo.DeclaringType?.GetCustomAttributes(true).OfType<AllowAnonymousAttribute>().Any() ?? false);

        if (hasAllowAnonymous)
            return;

        var hasAuthorize = context.MethodInfo.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any()
            || context.MethodInfo.GetCustomAttributes(true).OfType<RoleAuthorizeAttribute>().Any()
            || (context.MethodInfo.DeclaringType?.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any() ?? false)
            || (context.MethodInfo.DeclaringType?.GetCustomAttributes(true).OfType<RoleAuthorizeAttribute>().Any() ?? false);

        if (!hasAuthorize)
            return;

        operation.Security ??= new List<OpenApiSecurityRequirement>();

        var securityRequirement = new OpenApiSecurityRequirement
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
        };

        operation.Security.Add(securityRequirement);

        operation.Responses.TryAdd("401", new OpenApiResponse
        {
            Description = "Unauthorized — missing, malformed, expired or revoked access token. Log in (or refresh) to obtain a new one."
        });
        operation.Responses.TryAdd("403", new OpenApiResponse
        {
            Description = "Forbidden — valid token, but the account lacks the required role/permission or is deactivated."
        });
    }
}
