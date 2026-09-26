using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Common.Options;
using Stock_Exchange.Application.Features.Auth.DTOs;
using Stock_Exchange.Domain.Entities;
using Stock_Exchange.Domain.Repositories.Interfaces;
using Stock_Exchange.Domain.Repositories.Interfaces.Base;

namespace Stock_Exchange.Application.Features.Auth.Commands.RefreshToken
{
    public sealed class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, RefreshTokenResponseDto>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IUserRefreshTokenRepository _refreshTokenRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly JwtSettings _jwtSettings;

        public RefreshTokenCommandHandler(
            UserManager<ApplicationUser> userManager,
            IJwtTokenService jwtTokenService,
            IUserRefreshTokenRepository refreshTokenRepository,
            IUnitOfWork unitOfWork,
            IOptions<JwtSettings> jwtSettings)
        {
            _userManager = userManager;
            _jwtTokenService = jwtTokenService;
            _refreshTokenRepository = refreshTokenRepository;
            _unitOfWork = unitOfWork;
            _jwtSettings = jwtSettings.Value;
        }

        public async Task<RefreshTokenResponseDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var storedToken = (await _refreshTokenRepository
                .GetFirstAsync(x => x.Token == request.RefreshToken, cancellationToken))!;

            var user = (await _userManager.FindByIdAsync(storedToken.UserId.ToString()))!;

            user.IncrementTokenVersion();
            await _userManager.UpdateAsync(user);

            storedToken.Revoke();

            var roles = await _userManager.GetRolesAsync(user);
            var newAccessToken = _jwtTokenService.GenerateAccessToken(user, roles);
            var newRefreshToken = _jwtTokenService.GenerateRefreshToken(user);

            var userRefreshToken = UserRefreshToken.Create(
                user.Id,
                newRefreshToken,
                DateTime.Now.AddDays(_jwtSettings.RefreshTokenExpiryDays));

            await _refreshTokenRepository.AddAsync(userRefreshToken);
            await _unitOfWork.SaveChangesAsync();

            return new RefreshTokenResponseDto(newAccessToken, newRefreshToken);
        }
    }
}
