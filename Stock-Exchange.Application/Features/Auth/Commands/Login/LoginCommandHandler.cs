using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Stock_Exchange.Application.Common.Exceptions;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Common.Options;
using Stock_Exchange.Application.Features.Auth.DTOs;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Entities;
using Stock_Exchange.Domain.Repositories.Interfaces;
using Stock_Exchange.Domain.Repositories.Interfaces.Base;

namespace Stock_Exchange.Application.Features.Auth.Commands.Login
{
    public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponseDto>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IUserRefreshTokenRepository _refreshTokenRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly JwtSettings _jwtSettings;
        private readonly IStringLocalizer<Messages> _localizer;

        public LoginCommandHandler(
            UserManager<ApplicationUser> userManager,
            IJwtTokenService jwtTokenService,
            IUserRefreshTokenRepository refreshTokenRepository,
            IUnitOfWork unitOfWork,
            IOptions<JwtSettings> jwtSettings,
            IStringLocalizer<Messages> localizer)
        {
            _userManager = userManager;
            _jwtTokenService = jwtTokenService;
            _refreshTokenRepository = refreshTokenRepository;
            _unitOfWork = unitOfWork;
            _jwtSettings = jwtSettings.Value;
            _localizer = localizer;
        }

        public async Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user is null || !await _userManager.CheckPasswordAsync(user, request.Password))
                throw new UnAuthorizedException(_localizer[LocalizationKeys.AuthMessages.InvalidCredentials]);

            if (user.IsDeleted)
                throw new ForbiddenException(_localizer[LocalizationKeys.AuthMessages.AccountDeleted]);

            if (!user.IsActive)
                throw new ForbiddenException(_localizer[LocalizationKeys.AuthMessages.AccountDeactivated]);

            var roles = await _userManager.GetRolesAsync(user);
            var accessToken = _jwtTokenService.GenerateAccessToken(user, roles);

            var existingToken = await _refreshTokenRepository
                .GetFirstAsync(x => x.UserId == user.Id && !x.IsRevoked && !x.IsDeleted && x.IsActive && x.ExpiryDate > DateTime.Now, cancellationToken);

            string refreshToken;
            if (existingToken != null)
            {
                refreshToken = existingToken.Token;
            }
            else
            {
                var expiredTokens = await _refreshTokenRepository
                    .GetAllAsync(x => x.UserId == user.Id && (x.IsRevoked || x.ExpiryDate <= DateTime.Now))
                    .ToListAsync(cancellationToken);

                foreach (var token in expiredTokens)
                    token.Revoke();

                refreshToken = _jwtTokenService.GenerateRefreshToken(user);
                var userRefreshToken = UserRefreshToken.Create(
                    user.Id,
                    refreshToken,
                    DateTime.Now.AddDays(_jwtSettings.RefreshTokenExpiryDays));

                await _refreshTokenRepository.AddAsync(userRefreshToken);
                await _unitOfWork.SaveChangesAsync();
            }

            return new AuthResponseDto(
                accessToken,
                refreshToken,
                user.FullName,
                user.Email!,
                user.PhoneNumber ?? string.Empty,
                string.Join(",", roles),
                user.Id,
                user.ProfilePictureUrl);
        }
    }
}
