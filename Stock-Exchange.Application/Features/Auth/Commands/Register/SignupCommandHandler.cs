using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Stock_Exchange.Application.Common.Exceptions;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Common.Options;
using Stock_Exchange.Application.Features.Auth.Commands.Login;
using Stock_Exchange.Application.Features.Auth.DTOs;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Entities;
using Stock_Exchange.Domain.Enums;
using Stock_Exchange.Domain.Repositories.Interfaces;
using Stock_Exchange.Domain.Repositories.Interfaces.Base;

namespace Stock_Exchange.Application.Features.Auth.Commands.Register
{
    public sealed class SignupCommandHandler : IRequestHandler<SignupCommand, AuthResponseDto>
    {
        private readonly IMediator _mediator;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IUserRefreshTokenRepository _refreshTokenRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly JwtSettings _jwtSettings;
        private readonly IStringLocalizer<Messages> _localizer;

        public SignupCommandHandler(
            IMediator mediator,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole<Guid>> roleManager,
            IJwtTokenService jwtTokenService,
            IUserRefreshTokenRepository refreshTokenRepository,
            IUnitOfWork unitOfWork,
            IOptions<JwtSettings> jwtSettings,
            IStringLocalizer<Messages> localizer)
        {
            _mediator = mediator;
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtTokenService = jwtTokenService;
            _refreshTokenRepository = refreshTokenRepository;
            _unitOfWork = unitOfWork;
            _jwtSettings = jwtSettings.Value;
            _localizer = localizer;
        }

        public async Task<AuthResponseDto> Handle(SignupCommand request, CancellationToken cancellationToken)
        {
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                return await _mediator.Send(
                    new LoginCommand(request.Email, request.Password),
                    cancellationToken);
            }

            if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
            {
                var existingPhone = await _userManager.Users
                    .AnyAsync(u => u.PhoneNumber == request.PhoneNumber && !u.IsDeleted, cancellationToken);

                if (existingPhone)
                    throw new ConflictException(_localizer[LocalizationKeys.AuthMessages.PhoneNumberAlreadyExists]);
            }

            var user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber
            };
            user.UpdateFullName(request.FullName);

            var createResult = await _userManager.CreateAsync(user, request.Password);
            if (!createResult.Succeeded)
            {
                var errors = createResult.Errors
                    .GroupBy(e => e.Code)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.Description).ToArray());

                throw new BadRequestException(
                    errors,
                    LocalizationKeys.AuthMessages.UserCreationFailed);
            }

            var customerRole = UserType.Customer.ToString();
            if (!await _roleManager.RoleExistsAsync(customerRole))
            {
                await _roleManager.CreateAsync(new IdentityRole<Guid>(customerRole));
            }

            await _userManager.AddToRoleAsync(user, customerRole);

            var roles = new List<string> { customerRole };
            var accessToken = _jwtTokenService.GenerateAccessToken(user, roles);

            var refreshToken = _jwtTokenService.GenerateRefreshToken(user);
            var userRefreshToken = UserRefreshToken.Create(
                user.Id,
                refreshToken,
                DateTime.Now.AddDays(_jwtSettings.RefreshTokenExpiryDays));

            await _refreshTokenRepository.AddAsync(userRefreshToken);
            await _unitOfWork.SaveChangesAsync();

            return new AuthResponseDto(
                accessToken,
                refreshToken,
                user.FullName,
                user.Email!,
                user.PhoneNumber ?? string.Empty,
                customerRole,
                user.Id,
                user.ProfilePictureUrl);
        }
    }
}
