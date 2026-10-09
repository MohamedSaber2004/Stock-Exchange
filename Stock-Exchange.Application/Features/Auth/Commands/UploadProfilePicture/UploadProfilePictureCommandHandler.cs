using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Stock_Exchange.Application.Common.Exceptions;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Entities;

namespace Stock_Exchange.Application.Features.Auth.Commands.UploadProfilePicture
{
    public class UploadProfilePictureCommandHandler : IRequestHandler<UploadProfilePictureCommand, string>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICurrentUserService _currentUserService;
        private readonly IImageValidator _imageValidator;
        private readonly IStringLocalizer<Messages> _localizer;

        public UploadProfilePictureCommandHandler(
            UserManager<ApplicationUser> userManager,
            ICurrentUserService currentUserService,
            IImageValidator imageValidator,
            IStringLocalizer<Messages> localizer)
        {
            _userManager = userManager;
            _currentUserService = currentUserService;
            _imageValidator = imageValidator;
            _localizer = localizer;
        }

        public async Task<string> Handle(UploadProfilePictureCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUserService.IsAuthenticated || _currentUserService.UserId == Guid.Empty)
                throw new UnAuthorizedException(_localizer[LocalizationKeys.ExceptionMessages.Unauthorized]);

            var user = await _userManager.FindByIdAsync(_currentUserService.UserId.ToString());
            if (user == null || user.IsDeleted || !user.IsActive)
                throw new NotFoundException(_localizer[LocalizationKeys.AuthMessages.UserNotFound]);

            var (uploaded, result) = await _imageValidator.UploadImage(request.File, request.Place);
            if (!uploaded)
                throw new BadRequestException(result);

            if (!string.IsNullOrWhiteSpace(user.ProfilePictureUrl) &&
                !user.ProfilePictureUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
                !user.ProfilePictureUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                await _imageValidator.DeleteImage(user.ProfilePictureUrl, request.Place);
            }

            user.UpdateProfilePicture(result);
            user.MarkAsUpdated(_currentUserService.UserId.ToString());

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                var errors = updateResult.Errors
                    .GroupBy(e => e.Code)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.Description).ToArray());

                throw new BadRequestException(errors, LocalizationKeys.ExceptionMessages.BadRequest);
            }

            return result;
        }
    }
}
