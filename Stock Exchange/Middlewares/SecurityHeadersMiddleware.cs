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

            var options = GetEffectiveOptions();
            _securityHeadersService.AddSecurityHeaders(context, options);
        }

        private SecurityHeadersOptions GetEffectiveOptions()
        {
            if (!_env.IsDevelopment() || string.IsNullOrEmpty(_options.ContentSecurityPolicy))
                return _options;

            var csp = _options.ContentSecurityPolicy;
            var modified = false;

            if (csp.Contains("connect-src") && !csp.Contains("ws:"))
            {
                csp = csp.Replace("connect-src 'self'", "connect-src 'self' http://localhost:* https://localhost:* ws://localhost:* wss://localhost:* ws: wss:");
                modified = true;
            }

            if (csp.Contains("script-src") && !csp.Contains("localhost"))
            {
                csp = csp.Replace("script-src 'self'", "script-src 'self' http://localhost:* https://localhost:*");
                modified = true;
            }

            if (!modified)
                return _options;

            return new SecurityHeadersOptions
            {
                Enabled = _options.Enabled,
                XFrameOptions = _options.XFrameOptions,
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