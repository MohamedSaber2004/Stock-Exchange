using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Stock_Exchange.Application.Common.Interfaces;

namespace Stock_Exchange.Persistance
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
        {
            var defaultConnectionString = "Server=db69245.public.databaseasp.net; Database=db69245; User Id=db69245; Password=Mo@123456; Encrypt=True; TrustServerCertificate=True; MultipleActiveResultSets=True;";
            var connectionString = configuration.GetConnectionString("StockExchangeConnectionString")
                ?? configuration.GetConnectionString("CareClinicHubDb")
                ?? configuration["ConnectionStrings:StockExchangeConnectionString"]
                ?? defaultConnectionString;

            services.AddDbContext<StockExchangeDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });

            services.AddScoped<IStockExchangeDbContext>(provider => provider.GetRequiredService<StockExchangeDbContext>());

            return services;
        }
    }
}
