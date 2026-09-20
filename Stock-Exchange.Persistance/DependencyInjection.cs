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
            services.AddDbContext<StockExchangeDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("CareClinicHubDb"));
            });

            services.AddScoped<IStockExchangeDbContext>(provider => provider.GetRequiredService<StockExchangeDbContext>());

            return services;
        }
    }
}
