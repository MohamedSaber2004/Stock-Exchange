using Stock_Exchange.Application.Common.Interfaces;
using System.Security.Claims;

namespace Stock_Exchange.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        public Guid UserId { get; }

        public bool IsAuthenticated { get; }

        public string? IpAddress { get; }

        public int? UserTypes { get; }

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            var httpContext = httpContextAccessor.HttpContext;

            if (httpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value is { } userIdString &&
                Guid.TryParse(userIdString, out var userId))
            {
                UserId = userId;
            }
            else
            {
                UserId = Guid.Empty;
            }

            IsAuthenticated = httpContext?.User?.Identity?.IsAuthenticated ?? false;
            IpAddress = httpContext?.Connection?.RemoteIpAddress?.ToString();

            var userTypesClaim = httpContext?.User?.FindFirst("UserTypes")?.Value;
            if (int.TryParse(userTypesClaim, out var userTypes))
            {
                UserTypes = userTypes;
            }
        }
    }
}
