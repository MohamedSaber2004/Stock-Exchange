using Microsoft.Extensions.DependencyInjection;
using Stock_Exchange.Application.Common.Options;

namespace Stock_Exchange.Application.Common.Interfaces
{
    public interface IAntiforgeryService
    {
        void ConfigureAntiforgery(IServiceCollection services, AntiforgeryOptions options);
    }
}