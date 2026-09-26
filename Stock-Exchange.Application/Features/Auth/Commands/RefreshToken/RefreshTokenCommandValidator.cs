using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Entities;
using Stock_Exchange.Domain.Repositories.Interfaces;

namespace Stock_Exchange.Application.Features.Auth.Commands.RefreshToken
{
    public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
    {
        private readonly IUserRefreshTokenRepository _refreshTokenRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        // Per-request cache: validators are registered as Transient, so instance
        // state is safe and avoids re-querying the same token in every rule.
        private UserRefreshToken? _cachedToken;
        private bool _tokenLoaded;
        private ApplicationUser? _cachedOwner;
        private bool _ownerLoaded;

        public RefreshTokenCommandValidator(
            IUserRefreshTokenRepository refreshTokenRepository,
            UserManager<ApplicationUser> userManager)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _userManager = userManager;

            RuleFor(x => x.RefreshToken)
                .NotEmpty()
                .WithMessage(LocalizationKeys.AuthMessages.RefreshTokenRequired)
                .MustAsync(TokenExists)
                .WithMessage(LocalizationKeys.AuthMessages.InvalidRefreshToken)
                .MustAsync(TokenNotRevoked)
                .WithMessage(LocalizationKeys.AuthMessages.RefreshTokenRevoked)
                .MustAsync(TokenNotExpired)
                .WithMessage(LocalizationKeys.AuthMessages.RefreshTokenExpired)
                .MustAsync(TokenIsUsable)
                .WithMessage(LocalizationKeys.AuthMessages.InvalidRefreshToken)
                .MustAsync(TokenOwnerExists)
                .WithMessage(LocalizationKeys.AuthMessages.InvalidRefreshToken)
                .MustAsync(TokenOwnerNotDeleted)
                .WithMessage(LocalizationKeys.AuthMessages.AccountDeleted)
                .MustAsync(TokenOwnerIsActive)
                .WithMessage(LocalizationKeys.AuthMessages.AccountDeactivated);
        }

        private async Task<UserRefreshToken?> GetTokenAsync(string token, CancellationToken cancellationToken)
        {
            if (!_tokenLoaded)
            {
                _cachedToken = await _refreshTokenRepository.GetFirstAsync(x => x.Token == token, cancellationToken);
                _tokenLoaded = true;
            }

            return _cachedToken;
        }

        private async Task<ApplicationUser?> GetOwnerAsync(string token, CancellationToken cancellationToken)
        {
            if (!_ownerLoaded)
            {
                var storedToken = await GetTokenAsync(token, cancellationToken);
                if (storedToken is not null)
                    _cachedOwner = await _userManager.FindByIdAsync(storedToken.UserId.ToString());

                _ownerLoaded = true;
            }

            return _cachedOwner;
        }

        private async Task<bool> TokenExists(string token, CancellationToken cancellationToken)
        {
            return await GetTokenAsync(token, cancellationToken) is not null;
        }

        private async Task<bool> TokenNotRevoked(string token, CancellationToken cancellationToken)
        {
            var storedToken = await GetTokenAsync(token, cancellationToken);
            // Existence is already reported by TokenExists; skip further noise when missing.
            return storedToken is null || !storedToken.IsRevoked;
        }

        private async Task<bool> TokenNotExpired(string token, CancellationToken cancellationToken)
        {
            var storedToken = await GetTokenAsync(token, cancellationToken);
            return storedToken is null || storedToken.ExpiryDate > DateTime.Now;
        }

        private async Task<bool> TokenIsUsable(string token, CancellationToken cancellationToken)
        {
            var storedToken = await GetTokenAsync(token, cancellationToken);
            return storedToken is null || (!storedToken.IsDeleted && storedToken.IsActive);
        }

        private async Task<bool> TokenOwnerExists(string token, CancellationToken cancellationToken)
        {
            var storedToken = await GetTokenAsync(token, cancellationToken);
            if (storedToken is null)
                return true;

            return await GetOwnerAsync(token, cancellationToken) is not null;
        }

        private async Task<bool> TokenOwnerNotDeleted(string token, CancellationToken cancellationToken)
        {
            var owner = await GetOwnerAsync(token, cancellationToken);
            return owner is null || !owner.IsDeleted;
        }

        private async Task<bool> TokenOwnerIsActive(string token, CancellationToken cancellationToken)
        {
            var owner = await GetOwnerAsync(token, cancellationToken);
            return owner is null || owner.IsActive;
        }
    }
}
