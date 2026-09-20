using Stock_Exchange.Domain.Entities;

namespace Stock_Exchange.Application.Common.Interfaces
{
    public interface IJwtTokenService
    {
        string GenerateAccessToken(ApplicationUser user, IList<string> roles, Guid? clinicId = null, bool hasActiveSubscription = false);

        string GenerateRefreshToken(ApplicationUser user);
    }
}
