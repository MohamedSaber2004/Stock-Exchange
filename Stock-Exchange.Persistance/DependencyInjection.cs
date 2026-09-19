using Microsoft.Extensions.DependencyInjection;

namespace Stock_Exchange.Persistance
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistenceServices(this IServiceCollection services)
        {
            return services;
        }
    }
}
