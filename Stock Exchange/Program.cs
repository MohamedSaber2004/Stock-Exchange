
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.OpenApi.Models;
using Serilog;
using Stock_Exchange.Application;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Common.Options;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Infrastructure;
using Stock_Exchange.Middlewares;
using Stock_Exchange.Persistance;
using Stock_Exchange.Services;
using Stock_Exchange.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Globalization;
using System.Reflection;

namespace Stock_Exchange
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var env = builder.Environment;

            builder.Configuration.Sources.Clear();
            builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);

            JsonLocalizationProvider.Initialize(env.ContentRootPath);

            if (env.IsDevelopment() || env.EnvironmentName == "Test")
            {
                var appAssembly = Assembly.Load(new AssemblyName(env.ApplicationName));
                if (appAssembly != null) builder.Configuration.AddUserSecrets(appAssembly, optional: true);
            }

            builder.Configuration.AddEnvironmentVariables().AddCommandLine(args);
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .CreateBootstrapLogger();

            builder.Host.UseSerilog((context, services, configuration) => configuration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext());

            builder.Services.AddApplicationServices(builder.Configuration);
            builder.Services.AddInfrastructureServices(builder.Configuration);
            builder.Services.AddPersistenceServices(builder.Configuration);

            if (builder.Environment.IsProduction())
            {
                var corsConfig = builder.Configuration.GetSection("Security:Cors").Get<CorsOptions>();
                if (corsConfig?.Enabled == true)
                {
                    builder.Services.AddCors(options =>
                    {
                        options.AddPolicy(corsConfig.PolicyName, policy =>
                        {
                            if (corsConfig.AllowedOrigins != null && corsConfig.AllowedOrigins.Count > 0)
                                policy.WithOrigins(corsConfig.AllowedOrigins.ToArray());
                            else
                                policy.SetIsOriginAllowed(_ => true);

                            if (corsConfig.AllowedMethods != null && corsConfig.AllowedMethods.Count > 0)
                                policy.WithMethods(corsConfig.AllowedMethods.ToArray());
                            else
                                policy.AllowAnyMethod();

                            if (corsConfig.AllowedHeaders != null && corsConfig.AllowedHeaders.Count > 0)
                                policy.WithHeaders(corsConfig.AllowedHeaders.ToArray());
                            else
                                policy.AllowAnyHeader();

                            if (corsConfig.AllowCredentials)
                                policy.AllowCredentials();
                        });
                    });
                }
            }

            builder.Services.AddControllers()
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.InvalidModelStateResponseFactory = context =>
                    {
                        var localizationProvider = context.HttpContext.RequestServices.GetService<ILocalizationProvider>();
                        var culture = context.HttpContext.Request.Headers["Accept-Language"].FirstOrDefault() ?? "ar";
                        if (culture.Contains('-')) culture = culture.Split('-')[0];
                        if (culture.Length > 2) culture = culture.Substring(0, 2);
                        if (culture != "en" && culture != "ar") culture = "ar";

                        var message = localizationProvider?.GetLocalizedString(LocalizationKeys.ExceptionMessages.InvalidModelState, culture)
                                      ?? "The provided model state is invalid.";

                        var errors = context.ModelState
                            .Where(x => x.Value?.Errors.Count > 0)
                            .ToDictionary(
                                kvp => string.IsNullOrWhiteSpace(kvp.Key) ? "General" : kvp.Key,
                                kvp => kvp.Value!.Errors.Select(e =>
                                    !string.IsNullOrWhiteSpace(e.ErrorMessage)
                                        ? (localizationProvider?.GetLocalizedString(e.ErrorMessage, culture) ?? e.ErrorMessage)
                                        : (localizationProvider?.GetLocalizedString(LocalizationKeys.ExceptionMessages.InvalidModelState, culture) ?? "Invalid value.")
                                ).ToArray()
                            );

                        var response = ApiResponse<object?>.Error(errors, message, StatusCodes.Status400BadRequest);
                        return new BadRequestObjectResult(response);
                    };
                });
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddMemoryCache();
            builder.Services.AddInMemoryRateLimiting();
            builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
            builder.Services.Configure<ClientRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
            builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

            builder.Services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
            }).AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

            builder.Services.AddLocalization();
            builder.Services.AddSingleton<IStringLocalizerFactory, JsonStringLocalizerFactory>();
            builder.Services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[] { new CultureInfo("ar"), new CultureInfo("en") };
                options.DefaultRequestCulture = new RequestCulture("ar");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;

                options.RequestCultureProviders = new List<IRequestCultureProvider>
                    {
                        new AcceptLanguageHeaderRequestCultureProvider(),
                        new QueryStringRequestCultureProvider(),
                        new CookieRequestCultureProvider()
                    };
            });
            builder.Services.AddControllersWithViews().AddViewLocalization();

            builder.Services.AddSwaggerGen(options =>
            {
                options.AddAcceptLanguageHeader().IncludeApiXmlComments().AddJwtBearerSecurity();
            });

            builder.Services.AddOptions<SwaggerGenOptions>().Configure<IApiVersionDescriptionProvider>((options, provider) =>
            {
                options.AddVersionedSwaggerDocs(provider);
            });

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

            builder.Services.AddHsts(options =>
            {
                options.Preload = true;
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(365);
            });

            var app = builder.Build();

            app.UseCustomExceptionHandler();
            app.UseSecurityHeaders();

            app.UseRequestLocalization();

            if (app.Environment.EnvironmentName == "Production")
            {
                app.UseHsts();
            }

            app.UseRouting();

            if (app.Environment.IsProduction())
            {
                var corsSettings = app.Configuration.GetSection("Security:Cors").Get<CorsOptions>();
                if (corsSettings?.Enabled == true)
                {
                    app.UseCors(corsSettings.PolicyName);
                }
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseStaticFiles();

            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new CustomFileProvider(app.Environment.WebRootPath),
                RequestPath = "/files"
            }); ;

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
                foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
                {
                    c.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName);
                }
            });

            app.MapGet("/", () => Results.Redirect("/swagger/"));

            app.UseIpRateLimiting();

            app.MapControllers();

            try
            {
                app.Run();
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
