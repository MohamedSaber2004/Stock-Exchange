using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Stock_Exchange.Application.Common.Exceptions;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.Users.DTOs;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Entities;
using Stock_Exchange.Domain.Enums;
using Stock_Exchange.Domain.Repositories.Interfaces;
using Stock_Exchange.Domain.Repositories.Interfaces.Base;

namespace Stock_Exchange.Application.Features.Users.Commands.UpdateUser
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Result<UserDto>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly IUserRefreshTokenRepository _refreshTokenRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IStockExchangeDbContext _dbContext;
        private readonly ICurrentLanguageService _currentLanguageService;

        public UpdateUserCommandHandler(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole<Guid>> roleManager,
            IUserRefreshTokenRepository refreshTokenRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            IStockExchangeDbContext dbContext,
            ICurrentLanguageService currentLanguageService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _refreshTokenRepository = refreshTokenRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _dbContext = dbContext;
            _currentLanguageService = currentLanguageService;
        }

        public async Task<Result<UserDto>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.Id.ToString());
            if (user == null || user.IsDeleted)
                throw new NotFoundException(LocalizationKeys.AuthMessages.UserNotFound);

            // Security Rule: An admin cannot modify details of another admin
            var targetRoles = await _userManager.GetRolesAsync(user);
            var isTargetAdmin = targetRoles.Contains(UserType.Admin.ToString());
            if (isTargetAdmin && user.Id != _currentUserService.UserId)
            {
                throw new ForbiddenException(LocalizationKeys.AuthMessages.CannotModifyOtherAdmin);
            }

            var trimmedEmail = request.Email.Trim();
            if (!string.Equals(user.Email, trimmedEmail, StringComparison.OrdinalIgnoreCase))
            {
                var emailExists = await _userManager.Users
                    .IgnoreQueryFilters()
                    .AsNoTracking()
                    .AnyAsync(u => u.NormalizedEmail == trimmedEmail.ToUpperInvariant() && u.Id != user.Id, cancellationToken);

                if (emailExists)
                    throw new ConflictException(LocalizationKeys.AuthMessages.EmailAlreadyExists);

                user.Email = trimmedEmail;
                user.UserName = trimmedEmail;
                user.NormalizedEmail = trimmedEmail.ToUpperInvariant();
                user.NormalizedUserName = trimmedEmail.ToUpperInvariant();
            }

            user.UpdateFullName(request.FullName);
            user.PhoneNumber = string.IsNullOrWhiteSpace(request.PhoneNumber) ? null : request.PhoneNumber.Trim();
            user.CountryId = request.CountryId;

            // Handle active/inactive state change
            var wasActive = user.IsActive;
            if (wasActive && !request.IsActive)
            {
                user.Deactivate();

                var activeTokens = await _refreshTokenRepository
                    .GetAllAsync(x => x.UserId == user.Id && !x.IsRevoked)
                    .ToListAsync(cancellationToken);

                foreach (var token in activeTokens)
                {
                    token.Revoke();
                }

                user.IncrementTokenVersion();
            }
            else if (!wasActive && request.IsActive)
            {
                user.Activate();
            }

            // Handle Role change
            var currentRoles = await _userManager.GetRolesAsync(user);
            var desiredRole = request.UserType.ToString();
            var roleChanged = !currentRoles.Contains(desiredRole);

            if (roleChanged)
            {
                if (!await _roleManager.RoleExistsAsync(desiredRole))
                {
                    await _roleManager.CreateAsync(new IdentityRole<Guid>(desiredRole));
                }

                if (currentRoles.Any())
                {
                    await _userManager.RemoveFromRolesAsync(user, currentRoles);
                }

                await _userManager.AddToRoleAsync(user, desiredRole);
                user.IncrementTokenVersion();
            }

            var currentUserId = _currentUserService.UserId != Guid.Empty
                ? _currentUserService.UserId.ToString()
                : "Admin";

            user.MarkAsUpdated(currentUserId);
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                var errors = updateResult.Errors
                    .GroupBy(e => e.Code)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.Description).ToArray());

                throw new BadRequestException(errors, LocalizationKeys.ExceptionMessages.BadRequest);
            }

            await _unitOfWork.SaveChangesAsync();

            var countryArName = (string?)null;
            var countryEnName = (string?)null;
            var countryCode = (string?)null;
            if (user.CountryId.HasValue)
            {
                var country = await _dbContext.Countries
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Id == user.CountryId.Value, cancellationToken);
                countryArName = country?.CountryArName;
                countryEnName = country?.CountryEnName;
                countryCode = country?.Code;
            }

            var dto = new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email ?? string.Empty,
                PhoneNumber = user.PhoneNumber,
                ProfilePictureUrl = user.ProfilePictureUrl,
                UserType = request.UserType,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                CountryId = user.CountryId,
                CountryArName = countryArName,
                CountryEnName = countryEnName,
                CountryCode = countryCode
            };

            dto.ApplyLanguageFilter(_currentLanguageService.Language);

            return Result<UserDto>.Success(dto);
        }
    }
}
