using Google.Apis.Auth;
using Stock_Exchange.Domain.Entities;

namespace Stock_Exchange.Application.Common.Interfaces
{
    public interface IGoogleAuth
    {
        Task<GoogleJsonWebSignature.Payload?> ValidateGoogleTokenAsync(string idToken, string correlationId, CancellationToken cancellationToken);

        Task LinkGoogleAccountIfNeeded(ApplicationUser user, GoogleJsonWebSignature.Payload payload, string correlationId);

        Task UpdateUserInfoFromGoogle(ApplicationUser user, GoogleJsonWebSignature.Payload payload, string correlationId);
    }
}
