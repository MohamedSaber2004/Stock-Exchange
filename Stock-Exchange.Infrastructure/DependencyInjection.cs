using Microsoft.Extensions.DependencyInjection;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Infrastructure.Services.Attachment;
using Stock_Exchange.Infrastructure.Services.Security;

namespace Stock_Exchange.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            services.AddSingleton<ISecurityHeadersService, SecurityHeadersService>();
            services.AddSingleton<ICorsPolicyService, CorsPolicyService>();
            services.AddSingleton<IAntiforgeryService, AntiforgeryService>();
            services.AddSingleton<IHstsService, HstsService>();

            services.AddHttpClient();

            services.AddScoped<IBaseFileService, BaseFileService>();
            services.AddScoped<IAudioValidator, AudioValidator>();
            services.AddScoped<IFileValidator, FileValidator>();
            services.AddScoped<IImageValidator, ImageValidator>();
            services.AddScoped<IVideoValidator, VideoValidator>();

            return services;
        }
    }
}
