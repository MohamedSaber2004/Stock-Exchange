using Microsoft.AspNetCore.Http;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Common.Options;

namespace Stock_Exchange.Infrastructure.Services.Security
{
    public class SecurityHeadersService : ISecurityHeadersService
    {
        public void AddSecurityHeaders(HttpContext context, SecurityHeadersOptions options)
        {
            context.Response.Headers["X-Frame-Options"] = options.XFrameOptions;
            context.Response.Headers["X-Content-Type-Options"] = options.XContentTypeOptions;
            context.Response.Headers["X-XSS-Protection"] = options.XssProtection;
            context.Response.Headers["Content-Security-Policy"] = options.ContentSecurityPolicy;
            context.Response.Headers["Referrer-Policy"] = options.ReferrerPolicy;
            context.Response.Headers["Permissions-Policy"] = options.PermissionsPolicy;

            if (context.Request.IsHttps)
            {
                context.Response.Headers["Strict-Transport-Security"] = options.StrictTransportSecurity;
            }
        }
    }
}