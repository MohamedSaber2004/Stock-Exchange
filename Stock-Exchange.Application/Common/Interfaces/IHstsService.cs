using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Stock_Exchange.Application.Common.Options;

namespace Stock_Exchange.Application.Common.Interfaces
{
    public interface IHstsService
    {
        void ConfigureHsts(IServiceCollection services, HstsOptions options);
        void UseHsts(IApplicationBuilder app, HstsOptions options);
    }
}