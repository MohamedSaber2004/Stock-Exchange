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

        public SecurityHeadersMiddleware(
            RequestDelegate next,
            ILogger<SecurityHeadersMiddleware> logger,
            ISecurityHeadersService securityHeadersService,
            IOptions<SecurityHeadersOptions> options)
        {
            _next = next;
            _logger = logger;
            _securityHeadersService = securityHeadersService;
            _options = options.Value;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!_options.Enabled)
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

            _securityHeadersService.AddSecurityHeaders(context, _options);
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