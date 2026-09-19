using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Stock_Exchange.Application.Common.Options;

namespace Stock_Exchange.Application.Common.Interfaces
{
    public interface ICorsPolicyService
    {
        void ConfigureCors(IServiceCollection services, CorsOptions options);
        void UseCors(IApplicationBuilder app, string policyName);
    }
}