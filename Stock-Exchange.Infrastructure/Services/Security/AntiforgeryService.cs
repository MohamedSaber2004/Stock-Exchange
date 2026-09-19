using Microsoft.AspNetCore.Antiforgery;
using Microsoft.Extensions.DependencyInjection;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Common.Options;
using AntiforgeryOptions = Stock_Exchange.Application.Common.Options.AntiforgeryOptions;
using SameSiteMode = Microsoft.AspNetCore.Http.SameSiteMode;

namespace Stock_Exchange.Infrastructure.Services.Security
{
    public class AntiforgeryService : IAntiforgeryService
    {
        public void ConfigureAntiforgery(IServiceCollection services, AntiforgeryOptions options)
        {
            services.AddAntiforgery(antiforgeryOptions =>
            {
                antiforgeryOptions.HeaderName = options.HeaderName;
                antiforgeryOptions.Cookie.Name = options.CookieName;
                antiforgeryOptions.Cookie.HttpOnly = options.CookieHttpOnly;
                antiforgeryOptions.Cookie.SameSite = ParseSameSite(options.CookieSameSite);
            });
        }

        private static SameSiteMode ParseSameSite(string value)
        {
            return value.ToLowerInvariant() switch
            {
                "lax" => SameSiteMode.Lax,
                "strict" => SameSiteMode.Strict,
                "none" => SameSiteMode.None,
                _ => SameSiteMode.Strict
            };
        }
    }
}