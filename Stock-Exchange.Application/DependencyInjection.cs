using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Stock_Exchange.Application.Common.Behaviours;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Common.Options;
using Stock_Exchange.Application.Common.Services;
using Stock_Exchange.Application.Features.Attachments.Commands.DownloadFile;
using Stock_Exchange.Application.Features.Attachments.Commands.UpdateFile;
using Stock_Exchange.Application.Features.Attachments.Commands.UploadFile;
using Stock_Exchange.Application.Features.Attachments.Commands.UploadMultipleFiles;
using Stock_Exchange.Application.Features.Auth.Commands.Login;
using Stock_Exchange.Application.Features.Auth.Commands.Register;
using Stock_Exchange.Application.Localization;

namespace Stock_Exchange.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<SecurityOptions>(configuration.GetSection(SecurityOptions.SectionName));
            services.Configure<SecurityHeadersOptions>(configuration.GetSection("Security:Headers"));
            services.Configure<CorsOptions>(configuration.GetSection("Security:Cors"));
            services.Configure<AntiforgeryOptions>(configuration.GetSection("Security:Antiforgery"));
            services.Configure<RequestLimitsOptions>(configuration.GetSection("Security:RequestLimits"));
            services.Configure<HstsOptions>(configuration.GetSection("Security:Hsts"));
            services.Configure<IpRateLimitingOptions>(configuration.GetSection(IpRateLimitingOptions.SectionName));
            services.Configure<JwtSettings>(configuration.GetSection(nameof(JwtSettings)));
            services.Configure<IdentityOptions>(configuration.GetSection(nameof(IdentityOptions)));

            services.AddMediatR(typeof(DependencyInjection).Assembly);

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));

            services.AddTransient<IValidator<UploadFileCommand>, UploadFileCommandValidator>();
            services.AddTransient<IValidator<UploadMultipleFilesCommand>, UploadMultipleFilesCommandValidator>();
            services.AddTransient<IValidator<DownloadFileCommand>, DownloadFileCommandValidator>();
            services.AddTransient<IValidator<UpdateFileCommand>, UpdateFileCommandValidator>();
            services.AddTransient<IValidator<LoginCommand>, LoginCommandValidator>();
            services.AddTransient<IValidator<SignupCommand>, SignupCommandValidator>();

            services.AddSingleton<ILocalizationProvider, JsonLocalizationProvider>();

            UploadPaths.Configure(configuration);

            return services;
        }
    }
}
