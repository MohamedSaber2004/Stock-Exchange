using FluentValidation;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Repositories.Interfaces;

namespace Stock_Exchange.Application.Features.Auth.Commands.Logout
{
    public class LogoutCommandValidator : AbstractValidator<LogoutCommand>
    {
        private readonly IUserRefreshTokenRepository _refreshTokenRepository;
        private readonly ICurrentUserService _currentUserService;

        public LogoutCommandValidator(
            IUserRefreshTokenRepository refreshTokenRepository,
            ICurrentUserService currentUserService)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _currentUserService = currentUserService;

            When(x => x.RefreshToken != null, () =>
            {
                RuleFor(x => x.RefreshToken!)
                    .NotEmpty()
                    .WithMessage(LocalizationKeys.AuthMessages.RefreshTokenRequired)
                    .MustAsync(TokenBelongsToCurrentUser)
                    .WithMessage(LocalizationKeys.AuthMessages.InvalidRefreshToken);
            });
        }

        private async Task<bool> TokenBelongsToCurrentUser(string token, CancellationToken cancellationToken)
        {
            // Authentication itself is enforced by [Authorize] + handler (401).
            // Skip the ownership check for unauthenticated callers so the handler
            // can return 401 instead of a 400 validation error.
            if (!_currentUserService.IsAuthenticated || _currentUserService.UserId == Guid.Empty)
                return true;

            return await _refreshTokenRepository.ExistsAsync(
                x => x.Token == token && x.UserId == _currentUserService.UserId,
                cancellationToken);
        }
    }
}
