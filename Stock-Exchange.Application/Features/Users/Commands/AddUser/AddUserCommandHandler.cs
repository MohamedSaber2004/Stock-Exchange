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

namespace Stock_Exchange.Application.Features.Users.Commands.AddUser
{
    public class AddUserCommandHandler : IRequestHandler<AddUserCommand, Result<UserDto>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly ICurrentUserService _currentUserService;
        private readonly IStockExchangeDbContext _dbContext;
        private readonly ICurrentLanguageService _currentLanguageService;

        public AddUserCommandHandler(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole<Guid>> roleManager,
            ICurrentUserService currentUserService,
            IStockExchangeDbContext dbContext,
            ICurrentLanguageService currentLanguageService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _currentUserService = currentUserService;
            _dbContext = dbContext;
            _currentLanguageService = currentLanguageService;
        }

        public async Task<Result<UserDto>> Handle(AddUserCommand request, CancellationToken cancellationToken)
        {
            var user = new ApplicationUser
            {
                UserName = request.Email.Trim(),
                Email = request.Email.Trim(),
                PhoneNumber = string.IsNullOrWhiteSpace(request.PhoneNumber) ? null : request.PhoneNumber.Trim(),
                CountryId = request.CountryId
            };

            user.UpdateFullName(request.FullName);
            user.ConfirmEmail();

            var currentUserId = _currentUserService.UserId != Guid.Empty
                ? _currentUserService.UserId.ToString()
                : "Admin";

            user.MarkAsCreated(currentUserId);
            if (!request.IsActive)
            {
                user.Deactivate();
            }

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors
                    .GroupBy(e => e.Code)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.Description).ToArray());

                throw new BadRequestException(errors, LocalizationKeys.AuthMessages.UserCreationFailed);
            }

            var roleName = request.UserType.ToString();
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
            }
            await _userManager.AddToRoleAsync(user, roleName);

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
