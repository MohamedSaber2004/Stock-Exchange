using Microsoft.AspNetCore.Http;
using Stock_Exchange.Application.Common.Options;

namespace Stock_Exchange.Application.Common.Interfaces
{
    public interface ISecurityHeadersService
    {
        void AddSecurityHeaders(HttpContext context, SecurityHeadersOptions options);
    }
}