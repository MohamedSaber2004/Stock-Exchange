using Stock_Exchange.Application.Common.Interfaces;
using System.Security.Claims;

namespace Stock_Exchange.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

        public Guid UserId
        {
            get
            {
                var userIdString = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                    ?? User?.FindFirst("sub")?.Value;

                return Guid.TryParse(userIdString, out var userId)
                    ? userId
                    : Guid.Empty;
            }
        }

        public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

        public string? IpAddress => _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();

        public int? UserTypes
        {
            get
            {
                var userTypesClaim = User?.FindFirst("UserTypes")?.Value;
                return int.TryParse(userTypesClaim, out var userTypes)
                    ? userTypes
                    : null;
            }
        }
    }
}
