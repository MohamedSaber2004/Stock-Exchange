using MediatR;
using Microsoft.AspNetCore.Identity;
using Stock_Exchange.Application.Common.Exceptions;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Entities;
using Stock_Exchange.Domain.Enums;

namespace Stock_Exchange.Application.Features.Users.Commands.DeleteUser
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Result<bool>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserPurgeService _userPurgeService;

        public DeleteUserCommandHandler(
            UserManager<ApplicationUser> userManager,
            ICurrentUserService currentUserService,
            IUserPurgeService userPurgeService)
        {
            _userManager = userManager;
            _currentUserService = currentUserService;
            _userPurgeService = userPurgeService;
        }

        public async Task<Result<bool>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            if (_currentUserService.UserId == request.Id)
            {
                throw new BadRequestException(LocalizationKeys.AuthMessages.CannotDeleteSelf);
            }

            var user = await _userManager.FindByIdAsync(request.Id.ToString());
            if (user == null || user.IsDeleted)
                throw new NotFoundException(LocalizationKeys.AuthMessages.UserNotFound);
                
            var targetRoles = await _userManager.GetRolesAsync(user);
            var isTargetAdmin = targetRoles.Contains(UserType.Admin.ToString());
            if (isTargetAdmin)
            {
                throw new ForbiddenException(LocalizationKeys.AuthMessages.CannotModifyOtherAdmin);
            }

            return await _userPurgeService.PurgeUserAsync(user, cancellationToken);
        }
    }
}
