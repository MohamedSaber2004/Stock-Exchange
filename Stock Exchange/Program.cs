
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Localization;
using Microsoft.OpenApi.Models;
using Serilog;
using Stock_Exchange.Application;
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

            builder.Host.UseSerilog();

            builder.Services.AddApplicationServices(builder.Configuration);
            builder.Services.AddInfrastructureServices();
            builder.Services.AddPersistenceServices();

            builder.Services.AddControllers();
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
                options.AddAcceptLanguageHeader().IncludeApiXmlComments();
            });

            builder.Services.AddOptions<SwaggerGenOptions>().Configure<IApiVersionDescriptionProvider>((options, provider) =>
            {
                options.AddVersionedSwaggerDocs(provider);
            });

            builder.Services.AddHsts(options =>
            {
                options.Preload = true;
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(365);
            });

            var app = builder.Build();

            app.UseCustomExceptionHandler();

            app.UseRequestLocalization();

            if (app.Environment.EnvironmentName == "Production")
            {
                app.UseHsts();
            }

            app.UseRouting();
            app.UseHttpsRedirection();
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

            // Direct route for /swagger/index.html
            app.MapGet("/swagger/index.html", () => Results.Redirect("/swagger/"));

            app.UseIpRateLimiting();

            app.MapControllers();

            app.Run();
        }
    }
}
