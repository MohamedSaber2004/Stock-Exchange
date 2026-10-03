using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Common.Options;

namespace Stock_Exchange.Middlewares
{
    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<SecurityHeadersMiddleware> _logger;
        private readonly ISecurityHeadersService _securityHeadersService;
        private readonly SecurityHeadersOptions _options;
        private readonly IHostEnvironment _env;

        public SecurityHeadersMiddleware(
            RequestDelegate next,
            ILogger<SecurityHeadersMiddleware> logger,
            ISecurityHeadersService securityHeadersService,
            IOptions<SecurityHeadersOptions> options,
            IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _securityHeadersService = securityHeadersService;
            _options = options.Value;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!_options.Enabled)
            {
                await _next(context);
                return;
            }

            // Exclude Swagger and local dev tooling endpoints from security headers
            if (context.Request.Path.StartsWithSegments("/swagger") ||
                context.Request.Path.StartsWithSegments("/_vs") ||
                context.Request.Path.StartsWithSegments("/_framework"))
            {
                await _next(context);
                return;
            }

            try
            {
                AddSecurityHeaders(context);
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while applying security headers.");
                throw;
            }
        }

        private void AddSecurityHeaders(HttpContext context)
        {
            if (context.Response.HasStarted)
            {
                _logger.LogWarning("Security headers could not be added because the response has already started.");
                return;
            }

            var path = context.Request.Path.Value ?? string.Empty;
            var isViewEndpoint = path.EndsWith("/view", StringComparison.OrdinalIgnoreCase) ||
                                 path.Contains("/view/", StringComparison.OrdinalIgnoreCase);

            var options = GetEffectiveOptions(isViewEndpoint);
            _securityHeadersService.AddSecurityHeaders(context, options);
        }

        private SecurityHeadersOptions GetEffectiveOptions(bool isViewEndpoint)
        {
            var csp = _options.ContentSecurityPolicy ?? string.Empty;
            var xFrame = _options.XFrameOptions;

            if (isViewEndpoint)
            {
                xFrame = string.Empty;
                if (csp.Contains("frame-ancestors 'none'"))
                {
                    csp = csp.Replace("frame-ancestors 'none'", "frame-ancestors 'self' http://localhost:* https://localhost:* https://*.vercel.app https://*.runasp.net");
                }
            }

            if (_env.IsDevelopment() && !string.IsNullOrEmpty(csp))
            {
                if (csp.Contains("connect-src") && !csp.Contains("ws:"))
                {
                    csp = csp.Replace("connect-src 'self'", "connect-src 'self' http://localhost:* https://localhost:* ws://localhost:* wss://localhost:* ws: wss:");
                }

                if (csp.Contains("script-src") && !csp.Contains("localhost"))
                {
                    csp = csp.Replace("script-src 'self'", "script-src 'self' http://localhost:* https://localhost:*");
                }

                if (csp.Contains("frame-ancestors 'none'"))
                {
                    csp = csp.Replace("frame-ancestors 'none'", "frame-ancestors 'self' http://localhost:* https://localhost:* https://*.vercel.app https://*.runasp.net");
                }
            }

            return new SecurityHeadersOptions
            {
                Enabled = _options.Enabled,
                XFrameOptions = xFrame,
                XContentTypeOptions = _options.XContentTypeOptions,
                XssProtection = _options.XssProtection,
                ContentSecurityPolicy = csp,
                ReferrerPolicy = _options.ReferrerPolicy,
                PermissionsPolicy = _options.PermissionsPolicy,
                StrictTransportSecurity = _options.StrictTransportSecurity
            };
        }
    }

    public static class SecurityHeadersMiddlewareExtensions
    {
        public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder app)
        {
            return app.UseMiddleware<SecurityHeadersMiddleware>();
        }
    }
}
