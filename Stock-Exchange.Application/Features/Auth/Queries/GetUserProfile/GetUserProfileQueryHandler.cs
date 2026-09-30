using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.Auth.DTOs;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Entities;

namespace Stock_Exchange.Application.Features.Auth.Queries.GetUserProfile
{
    public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, Result<UserProfileDto>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICurrentUserService _currentUserService;

        public GetUserProfileQueryHandler(
            UserManager<ApplicationUser> userManager,
            ICurrentUserService currentUserService)
        {
            _userManager = userManager;
            _currentUserService = currentUserService;
        }

        public async Task<Result<UserProfileDto>> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
        {
            if (!_currentUserService.IsAuthenticated || _currentUserService.UserId == Guid.Empty)
                return Result<UserProfileDto>.Failure(LocalizationKeys.ExceptionMessages.Unauthorized, StatusCodes.Status401Unauthorized);

            var userId = _currentUserService.UserId;

            var profile = await _userManager.Users
                .AsNoTracking()
                .Where(u => u.Id == userId && !u.IsDeleted && u.IsActive)
                .Select(u => new UserProfileDto(
                    u.Id,
                    u.FullName,
                    u.Email ?? string.Empty,
                    u.PhoneNumber ?? string.Empty,
                    u.Country != null ? u.Country.Code : null,
                    u.ProfilePictureUrl,
                    u.Language))
                .FirstOrDefaultAsync(cancellationToken);

            if (profile is null)
                return Result<UserProfileDto>.Failure(LocalizationKeys.AuthMessages.UserNotFound, StatusCodes.Status404NotFound);

            return Result<UserProfileDto>.Success(profile);
        }
    }
}
