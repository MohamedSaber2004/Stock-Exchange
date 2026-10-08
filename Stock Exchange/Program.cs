using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.OpenApi.Models;
using NET_Tracker.Extensions;
using Serilog;
using Stock_Exchange.Application;
using Stock_Exchange.Application.Common.Converters;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Common.Options;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Entities;
using Stock_Exchange.Infrastructure;
using Stock_Exchange.Middlewares;
using Stock_Exchange.Persistance;
using Stock_Exchange.Persistance.Seeding;
using Stock_Exchange.Services;
using Stock_Exchange.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Globalization;
using System.Reflection;

namespace Stock_Exchange
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var env = builder.Environment;

            Directory.SetCurrentDirectory(env.ContentRootPath);
            var logsPath = Path.Combine(env.ContentRootPath, "Logs");
            if (!Directory.Exists(logsPath))
            {
                Directory.CreateDirectory(logsPath);
            }

            Serilog.Debugging.SelfLog.Enable(msg =>
            {
                try
                {
                    File.AppendAllText(Path.Combine(logsPath, "serilog-selflog.txt"), $"{DateTime.UtcNow:O} {msg}\n");
                }
                catch { }
            });

            builder.Configuration.Sources.Clear();
            builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile("appsettings.Public.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);

            JsonLocalizationProvider.Initialize(env.ContentRootPath);

            if (env.IsDevelopment() || env.EnvironmentName == "Test")
            {
                var appAssembly = Assembly.Load(new AssemblyName(env.ApplicationName));
                if (appAssembly != null) builder.Configuration.AddUserSecrets(appAssembly, optional: true);
            }

            var logFilePath = Path.Combine(logsPath, "log-.txt");

            builder.Configuration.AddEnvironmentVariables().AddCommandLine(args);
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
                .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File(
                    path: logFilePath,
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 31,
                    shared: true,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
                )
                .ReadFrom.Configuration(builder.Configuration)
                .CreateBootstrapLogger();

            builder.Host.UseSerilog((context, services, configuration) => configuration
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
                .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File(
                    path: logFilePath,
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 31,
                    shared: true,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
                )
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services));

            builder.Services.AddApplicationServices(builder.Configuration);
            builder.Services.AddInfrastructureServices(builder.Configuration);
            builder.Services.AddPersistenceServices(builder.Configuration);

            const string defaultDbConnectionString = "Server=db69245.public.databaseasp.net; Database=db69245; User Id=db69245; Password=Mo@123456; Encrypt=True; TrustServerCertificate=True; MultipleActiveResultSets=True;";

            var netTrackerConn = builder.Configuration.GetConnectionString("StockExchangeConnectionString")
                ?? builder.Configuration.GetConnectionString("DefaultConnection")
                ?? builder.Configuration["ConnectionStrings:StockExchangeConnectionString"]
                ?? builder.Configuration["ConnectionStrings:DefaultConnection"]
                ?? builder.Configuration["NetTracker:Storage:ConnectionString"]
                ?? builder.Configuration["HttpRequestResponseLogging:Storage:ConnectionString"]
                ?? defaultDbConnectionString;

            var isNetTrackerEnabled = builder.Configuration.GetValue<bool?>("NetTracker:Enabled") ?? true;
            var hasNetTracker = isNetTrackerEnabled && !string.IsNullOrWhiteSpace(netTrackerConn);

            if (hasNetTracker)
            {
                // Ensure NetTracker finds its expected connection string key
                builder.Configuration["ConnectionStrings:DefaultConnection"] ??= netTrackerConn;
                builder.Configuration["NetTracker:Storage:ConnectionString"] ??= netTrackerConn;
                builder.Configuration["NetTracker:Enabled"] = "true";
                builder.Configuration["NetTracker:EnableDashboardUI"] = "true";
                builder.Configuration["NetTracker:AllowRemoteDashboardAccess"] = "true";

                builder.Services.AddNetTracker(builder.Configuration);
                builder.Services.PostConfigure<NET_Tracker.Configuration.HttpLoggingOptions>(options =>
                {
                    options.Enabled = true;
                    options.EnableDashboardUI = true;
                    options.AllowRemoteDashboardAccess = true;

                    // Performance Optimization:
                    // 1. Avoid response body buffering on successful requests (eliminates memory pressure and latency)
                    options.LogRequestBody = true;
                    options.LogResponseBody = false;
                    options.LogBodyOnlyOnErrors = true;
                    options.LogHeaders = false;
                    options.MaxBodySize = 8192; // Limit payload capture to 8KB

                    // 2. Exclude logger's own endpoints, Swagger, and static assets to prevent self-logging loops
                    options.ExcludePaths ??= new List<string>();
                    var exclusions = new[]
                    {
                        "/health",
                        "/swagger",
                        "/swagger-custom",
                        "/favicon.ico",
                        "/net-tracker",
                        "/api/Statistics",
                        "/api/HttpTransactions",
                        "/files"
                    };
                    foreach (var path in exclusions)
                    {
                        if (!options.ExcludePaths.Contains(path, StringComparer.OrdinalIgnoreCase))
                        {
                            options.ExcludePaths.Add(path);
                        }
                    }

                    // 3. Keep database retention light (7 days) for high-speed queries on shared DB
                    if (options.Retention != null)
                    {
                        options.Retention.DaysToKeep = 7;
                        options.Retention.AutoCleanup = true;
                    }

                    // 4. Ensure non-blocking async queue
                    if (options.Performance != null)
                    {
                        options.Performance.UseAsyncLogging = true;
                        options.Performance.EnableCaching = true;
                        options.Performance.MaxQueueSize = 10000;
                    }
                });
            }
            else
            {
                Log.Information("NetTracker is disabled or no connection string configured; skipping NetTracker registration.");
            }

            var corsConfig = builder.Configuration.GetSection("Security:Cors").Get<CorsOptions>();
            var policyName = !string.IsNullOrWhiteSpace(corsConfig?.PolicyName) ? corsConfig.PolicyName : "DefaultPolicy";
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.SetIsOriginAllowed(_ => true)
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials();
                });
                options.AddPolicy(policyName, policy =>
                {
                    policy.SetIsOriginAllowed(_ => true)
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials();
                });
            });

            builder.Services.AddControllersWithViews()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new IsoUtcDateTimeConverter());
                    options.JsonSerializerOptions.Converters.Add(new IsoUtcNullableDateTimeConverter());
                    options.JsonSerializerOptions.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(System.Text.Unicode.UnicodeRanges.All);
                })
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

            builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

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
                options.AddAcceptLanguageHeader().IncludeApiXmlComments().AddJwtBearerSecurity().AddApiResponseExamples();
            });

            builder.Services.AddOptions<SwaggerGenOptions>().Configure<IApiVersionDescriptionProvider>((options, provider) =>
            {
                options.AddVersionedSwaggerDocs(provider);
            });

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
            builder.Services.AddScoped<ICurrentLanguageService, CurrentLanguageService>();

            builder.Services.AddHsts(options =>
            {
                options.Preload = true;
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(365);
            });

            var app = builder.Build();

            var forwardedHeadersOptions = new ForwardedHeadersOptions
            {
                ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.All
            };
            forwardedHeadersOptions.KnownNetworks.Clear();
            forwardedHeadersOptions.KnownProxies.Clear();
            app.UseForwardedHeaders(forwardedHeadersOptions);

            app.UseCustomExceptionHandler();
            app.UseSecurityHeaders();

            app.UseRequestLocalization();

            if (app.Environment.EnvironmentName == "Production")
            {
                app.UseHsts();
            }

            // Static files served first to completely bypass logger for CSS, JS, Images, etc.
            app.UseStaticFiles();

            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new CustomFileProvider(app.Environment.WebRootPath),
                RequestPath = "/files"
            });

            if (hasNetTracker)
            {
                // NET-Tracker Middleware (registered before UseRouting)
                app.UseNetTracker(app.Configuration);
            }

            app.UseRouting();

            var corsSettings = app.Configuration.GetSection("Security:Cors").Get<CorsOptions>();
            var activePolicy = !string.IsNullOrWhiteSpace(corsSettings?.PolicyName) ? corsSettings.PolicyName : "DefaultPolicy";
            app.UseCors(activePolicy);

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.InjectStylesheet("/swagger-custom/response-codes.css");
                c.InjectJavascript("/swagger-custom/response-codes.js");

                var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
                foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
                {
                    c.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName);
                }
            });

            app.MapGet("/", () => Results.Redirect("/swagger/"));

            app.UseIpRateLimiting();

            if (hasNetTracker)
            {
                // NET-Tracker MVC Routes for Logger Dashboard UI
                app.MapControllerRoute(
                    name: "nettracker_dashboard",
                    pattern: "net-tracker/dashboard/{action=Index}/{id?}",
                    defaults: new { controller = "Tracker", action = "Index" });

                app.MapGet("/net-tracker/dashboard", async context =>
                {
                    context.Response.ContentType = "text/html; charset=utf-8";
                    var webRoot = app.Environment.WebRootPath ?? Path.Combine(app.Environment.ContentRootPath, "wwwroot");
                    var staticHtml = Path.Combine(webRoot, "net-tracker", "index.html");
                    if (File.Exists(staticHtml))
                    {
                        await context.Response.SendFileAsync(staticHtml);
                        return;
                    }
                    context.Response.Redirect("/Tracker/Index");
                });

                app.MapGet("/net-tracker", () => Results.Redirect("/net-tracker/dashboard"));
                app.MapGet("/dashboard", () => Results.Redirect("/net-tracker/dashboard"));
            }

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapControllers();

            if (hasNetTracker)
            {
                // Ensure NetTracker database tables are created
                try
                {
                    using var trackerScope = app.Services.CreateScope();
                    var trackerDb = trackerScope.ServiceProvider.GetService<NET_Tracker.Data.ApplicationDbContext>();
                    if (trackerDb != null)
                    {
                        var tableExists = false;
                        try
                        {
                            await trackerDb.Database.ExecuteSqlRawAsync("SELECT TOP 1 1 FROM [HttpTransactions]");
                            tableExists = true;
                        }
                        catch
                        {
                            tableExists = false;
                        }

                        if (!tableExists)
                        {
                            var script = trackerDb.Database.GenerateCreateScript();
                            await trackerDb.Database.ExecuteSqlRawAsync(script);
                            Log.Information("=== NetTracker Database Tables Created Successfully ===");
                        }
                        else
                        {
                            Log.Information("=== NetTracker Database Tables Verified Successfully ===");
                        }

                        // Optimize table indexes for blazing fast dashboard queries
                        try
                        {
                            await trackerDb.Database.ExecuteSqlRawAsync(@"
                                IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'HttpTransactions')
                                AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_HttpTransactions_Perf' AND object_id = OBJECT_ID('HttpTransactions'))
                                BEGIN
                                    CREATE NONCLUSTERED INDEX IX_HttpTransactions_Perf 
                                    ON [HttpTransactions] ([Timestamp] DESC) 
                                    INCLUDE ([StatusCode], [Method], [DurationMs], [Url]);
                                END");
                            Log.Information("=== NetTracker Performance Index Created/Verified Successfully ===");
                        }
                        catch (Exception idxEx)
                        {
                            Log.Warning(idxEx, "Notice: Could not automatically create performance index on HttpTransactions table.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Warning(ex, "Could not automatically initialize NetTracker database tables.");
                }
            }

            // تشتغل بس لو شغّلت البروجيكت بـ: dotnet run --seed
            if (args.Contains("--seed"))
            {
                Log.Information("=== Running Database Seeding ===");
                using var scope = app.Services.CreateScope();
                var services = scope.ServiceProvider;
                try
                {
                    var dbContext = services.GetRequiredService<StockExchangeDbContext>();
                    var userManager = services.GetService<UserManager<ApplicationUser>>();
                    var roleManager = services.GetService<RoleManager<IdentityRole<Guid>>>();
                    await DataSeeder.SeedAllAsync(dbContext, userManager, roleManager);
                    Log.Information("=== Database Seeding Completed Successfully ===");
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "An error occurred while seeding the database.");
                }
            }

            try
            {
                Log.Information("=== Stock Exchange API Started === Environment: {Environment}, BaseDir: {BaseDir}, ContentRoot: {ContentRoot}", 
                    app.Environment.EnvironmentName, AppContext.BaseDirectory, app.Environment.ContentRootPath);
                await app.RunAsync();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Stock Exchange API terminated unexpectedly during runtime.");
                throw;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
