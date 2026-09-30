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

namespace Stock_Exchange.Application.Features.Users.Queries.GetUserById
{
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Result<UserDetailsDto>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IStockExchangeDbContext _dbContext;
        private readonly ICurrentLanguageService _currentLanguageService;

        public GetUserByIdQueryHandler(
            UserManager<ApplicationUser> userManager,
            IStockExchangeDbContext dbContext,
            ICurrentLanguageService currentLanguageService)
        {
            _userManager = userManager;
            _dbContext = dbContext;
            _currentLanguageService = currentLanguageService;
        }

        public async Task<Result<UserDetailsDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users
                .AsNoTracking()
                .Include(u => u.Country)
                .FirstOrDefaultAsync(u => u.Id == request.Id && !u.IsDeleted, cancellationToken);

            if (user == null)
                throw new NotFoundException(LocalizationKeys.AuthMessages.UserNotFound);

            var roles = await _userManager.GetRolesAsync(user);
            var isUserAdmin = roles.Any(r => string.Equals(r, UserType.Admin.ToString(), StringComparison.OrdinalIgnoreCase));
            var userType = isUserAdmin ? UserType.Admin : UserType.Customer;

            var dto = new UserDetailsDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email ?? string.Empty,
                PhoneNumber = user.PhoneNumber,
                ProfilePictureUrl = user.ProfilePictureUrl,
                UserType = userType,
                IsActive = user.IsActive,
                EmailConfirmed = user.EmailConfirmed,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                CountryId = user.CountryId,
                CountryArName = user.Country?.CountryArName,
                CountryEnName = user.Country?.CountryEnName,
                CountryCode = user.Country?.Code,
                Language = user.Language
            };

            dto.ApplyLanguageFilter(_currentLanguageService.Language);

            return Result<UserDetailsDto>.Success(dto);
        }
    }
}
