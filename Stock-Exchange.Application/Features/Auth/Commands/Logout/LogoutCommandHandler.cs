using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Stock_Exchange.Application.Common.Exceptions;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Entities;
using Stock_Exchange.Domain.Repositories.Interfaces;
using Stock_Exchange.Domain.Repositories.Interfaces.Base;

namespace Stock_Exchange.Application.Features.Auth.Commands.Logout
{
    public sealed class LogoutCommandHandler : IRequestHandler<LogoutCommand, bool>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserRefreshTokenRepository _refreshTokenRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IStringLocalizer<Messages> _localizer;

        public LogoutCommandHandler(
            UserManager<ApplicationUser> userManager,
            IUserRefreshTokenRepository refreshTokenRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            IStringLocalizer<Messages> localizer)
        {
            _userManager = userManager;
            _refreshTokenRepository = refreshTokenRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _localizer = localizer;
        }

        public async Task<bool> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUserService.IsAuthenticated || _currentUserService.UserId == Guid.Empty)
                throw new UnAuthorizedException(_localizer[LocalizationKeys.ExceptionMessages.Unauthorized]);

            var userId = _currentUserService.UserId;

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user is not null)
            {
                user.IncrementTokenVersion();
                await _userManager.UpdateAsync(user);
            }

            if (!string.IsNullOrWhiteSpace(request.RefreshToken))
            {
                var storedToken = (await _refreshTokenRepository
                    .GetFirstAsync(x => x.Token == request.RefreshToken && x.UserId == userId, cancellationToken))!;

                if (!storedToken.IsRevoked)
                {
                    storedToken.Revoke();
                    await _unitOfWork.SaveChangesAsync();
                }

                return true;
            }

            var activeTokens = await _refreshTokenRepository
                .GetAllAsync(x => x.UserId == userId && !x.IsRevoked)
                .ToListAsync(cancellationToken);

            foreach (var token in activeTokens)
                token.Revoke();

            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
