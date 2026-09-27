using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Domain.Entities;

namespace Stock_Exchange.Application.Common.Interfaces
{
    public interface IGoogleAuth
    {
        Task<GoogleUserProfile?> ValidateGoogleTokenAsync(string idToken, string correlationId, CancellationToken cancellationToken);

        Task LinkGoogleAccountIfNeeded(ApplicationUser user, GoogleUserProfile profile, string correlationId);

        Task UpdateUserInfoFromGoogle(ApplicationUser user, GoogleUserProfile profile, string correlationId);
    }
}
