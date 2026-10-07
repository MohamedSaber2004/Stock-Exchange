using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Stock_Exchange.Application.Common.Exceptions;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Entities;
using Stock_Exchange.Domain.Repositories.Interfaces;
using Stock_Exchange.Domain.Repositories.Interfaces.Base;

namespace Stock_Exchange.Application.Features.Auth.Commands.DeleteAccount
{
    public sealed class DeleteAccountCommandHandler : IRequestHandler<DeleteAccountCommand, bool>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserRefreshTokenRepository _refreshTokenRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteAccountCommandHandler(
            UserManager<ApplicationUser> userManager,
            ICurrentUserService currentUserService,
            IUserRefreshTokenRepository refreshTokenRepository,
            IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _currentUserService = currentUserService;
            _refreshTokenRepository = refreshTokenRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUserService.IsAuthenticated || _currentUserService.UserId == Guid.Empty)
                throw new UnAuthorizedException(LocalizationKeys.ExceptionMessages.Unauthorized);

            var user = await _userManager.FindByIdAsync(_currentUserService.UserId.ToString());
            if (user == null || user.IsDeleted)
                throw new NotFoundException(LocalizationKeys.AuthMessages.UserNotFound);

            var deletedBy = user.Email ?? user.Id.ToString();

            user.MarkAsDeleted(deletedBy);
            user.IncrementTokenVersion();

            var activeTokens = await _refreshTokenRepository
                .GetAllAsync(x => x.UserId == user.Id && !x.IsRevoked)
                .ToListAsync(cancellationToken);

            foreach (var token in activeTokens)
            {
                token.Revoke();
            }

            await _unitOfWork.SaveChangesAsync();

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                var errors = updateResult.Errors
                    .GroupBy(e => e.Code)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.Description).ToArray());

                throw new BadRequestException(errors, LocalizationKeys.ExceptionMessages.BadRequest);
            }

            return true;
        }
    }
}
