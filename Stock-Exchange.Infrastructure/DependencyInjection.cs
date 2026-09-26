using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Stock_Exchange.Application.Common.Auth;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Common.Options;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Entities;
using Stock_Exchange.Domain.Repositories.Interfaces;
using Stock_Exchange.Domain.Repositories.Interfaces.Base;
using Stock_Exchange.Infrastructure.Repositories.Implementations;
using Stock_Exchange.Infrastructure.Repositories.Implementations.Base;
using Stock_Exchange.Infrastructure.Services;
using Stock_Exchange.Infrastructure.Services.Attachment;
using Stock_Exchange.Infrastructure.Services.Email;
using Stock_Exchange.Infrastructure.Services.Security;
using Stock_Exchange.Persistance;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Stock_Exchange.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
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
            services.AddScoped<IJwtTokenService, JwtTokenService>();
            services.AddScoped<IEmailService, EmailService>();

            services.Configure<EmailSettings>(configuration.GetSection(nameof(EmailSettings)));
            var emailSettings = configuration.GetSection(nameof(EmailSettings)).Get<EmailSettings>() ?? new EmailSettings();
            services.AddSingleton(emailSettings);

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));
            services.AddScoped<IUserRefreshTokenRepository, UserRefreshTokenRepository>();

            services.Configure<Application.Common.Options.IdentityOptions>(configuration.GetSection("IdentityOptions"));
            var identityOptionsConfig = configuration.GetSection("IdentityOptions").Get<Application.Common.Options.IdentityOptions>()
                ?? new Application.Common.Options.IdentityOptions();
            services.AddSingleton(identityOptionsConfig);

            services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
            {
                options.Password.RequireNonAlphanumeric = identityOptionsConfig.RequireNonAlphanumeric;
                options.Password.RequiredLength = identityOptionsConfig.RequiredLength > 0 ? identityOptionsConfig.RequiredLength : 6;
                options.Password.RequireDigit = identityOptionsConfig.RequiredDigit;
                options.Password.RequireLowercase = identityOptionsConfig.RequireLowercase;
                options.Password.RequireUppercase = identityOptionsConfig.RequireUppercase;
                options.Lockout.MaxFailedAccessAttempts = identityOptionsConfig.MaxFailedAttempts > 0 ? identityOptionsConfig.MaxFailedAttempts : 5;
                options.SignIn.RequireConfirmedEmail = identityOptionsConfig.RequireConfirmedEmail;
                if (!string.IsNullOrWhiteSpace(identityOptionsConfig.AllowedUserNameCharacters))
                {
                    options.User.AllowedUserNameCharacters = identityOptionsConfig.AllowedUserNameCharacters;
                }
                options.User.RequireUniqueEmail = identityOptionsConfig.RequireUniqueEmail;
            })
           .AddEntityFrameworkStores<StockExchangeDbContext>()
           .AddDefaultTokenProviders();

            services.Configure<JwtSettings>(configuration.GetSection(nameof(JwtSettings)));
            var jwtSettings = configuration.GetSection(nameof(JwtSettings)).Get<JwtSettings>() ?? new JwtSettings();
            services.AddSingleton(jwtSettings);

            var secretKey = !string.IsNullOrWhiteSpace(jwtSettings.Secret)
                ? Encoding.UTF8.GetBytes(jwtSettings.Secret)
                : Encoding.UTF8.GetBytes("n]:#J:?,{%9SvotDc^+/FMs7XHl$R1D2c^,Sf7_6vGJ>L8^!WvK1$$BqjVjD}rHGp}[fxYa90K1%4l3yf;sx5:");

            var audiences = !string.IsNullOrWhiteSpace(jwtSettings.Audience)
                ? jwtSettings.Audience.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                : Array.Empty<string>();

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(secretKey),
                ValidateIssuer = !string.IsNullOrWhiteSpace(jwtSettings.Issuer),
                ValidIssuer = jwtSettings.Issuer,
                ValidateAudience = audiences.Length > 0,
                ValidAudiences = audiences.Length > 0 ? audiences : null,
                ValidAudience = audiences.Length == 1 ? audiences[0] : null,
                RequireExpirationTime = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
            };
            services.AddSingleton(tokenValidationParameters);

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.TokenValidationParameters = tokenValidationParameters;
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception is SecurityTokenExpiredException)
                        {
                            context.HttpContext.Items[AuthFailureReasons.ItemsKey] = AuthFailureReasons.Expired;
                        }

                        return Task.CompletedTask;
                    },
                    OnChallenge = context =>
                    {
                        context.HandleResponse();
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";

                        var localizedMessage = ResolveChallengeMessage(context);
                        var response = ApiResponse<object?>.Error(new Dictionary<string, string[]>(), localizedMessage, StatusCodes.Status401Unauthorized);
                        var jsonOptions = new System.Text.Json.JsonSerializerOptions
                        {
                            PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
                        };

                        return context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response, jsonOptions));
                    },
                    OnTokenValidated = async context =>
                    {
                        var userManager = context.HttpContext.RequestServices.GetService<UserManager<ApplicationUser>>();
                        var userId = context.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                            ?? context.Principal?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
                        var versionClaim = context.Principal?.FindFirst("TokenVersion")?.Value;

                        if (userManager is null
                            || !Guid.TryParse(userId, out _)
                            || !long.TryParse(versionClaim, out var tokenVersion))
                        {
                            context.HttpContext.Items[AuthFailureReasons.ItemsKey] = AuthFailureReasons.Revoked;
                            context.Fail(MissingTokenClaimsMessage);
                            return;
                        }

                        var user = await userManager.FindByIdAsync(userId!);
                        if (user is null
                            || user.IsDeleted
                            || !user.IsActive
                            || user.TokenVersion != tokenVersion)
                        {
                            context.HttpContext.Items[AuthFailureReasons.ItemsKey] = AuthFailureReasons.Revoked;
                            context.Fail(RevokedAccessTokenMessage);
                        }
                    }
                };
            });

            return services;
        }

        private const string MissingTokenClaimsMessage = "Access token is missing required claims.";
        private const string RevokedAccessTokenMessage = "Access token has been revoked.";

        private static string ResolveChallengeMessage(JwtBearerChallengeContext context)
        {
            var reason = context.HttpContext.Items[AuthFailureReasons.ItemsKey] as string;
            if (reason == AuthFailureReasons.Expired)
                return JsonLocalizationProvider.GetLocalizedString(LocalizationKeys.AuthMessages.SessionExpired);

            if (reason == AuthFailureReasons.Revoked)
                return JsonLocalizationProvider.GetLocalizedString(LocalizationKeys.AuthMessages.SessionRevoked);

            return context.AuthenticateFailure switch
            {
                SecurityTokenExpiredException => JsonLocalizationProvider.GetLocalizedString(
                    LocalizationKeys.AuthMessages.SessionExpired),
                AuthenticationFailureException authFailure
                    when authFailure.Message == RevokedAccessTokenMessage
                        || authFailure.Message == MissingTokenClaimsMessage => JsonLocalizationProvider.GetLocalizedString(
                            LocalizationKeys.AuthMessages.SessionRevoked),
                _ => JsonLocalizationProvider.GetLocalizedString(
                    LocalizationKeys.ExceptionMessages.Unauthorized)
            };
        }
    }
}
