using Google.Apis.Auth;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stock_Exchange.Application.Common.Exceptions;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Common.Options;
using Stock_Exchange.Application.Features.Attachments.Commands.UploadFile;
using Stock_Exchange.Application.Features.Auth.DTOs;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Entities;
using Stock_Exchange.Domain.Enums;
using Stock_Exchange.Domain.Repositories.Interfaces;
using Stock_Exchange.Domain.Repositories.Interfaces.Base;

namespace Stock_Exchange.Application.Features.Auth.Commands.LoginWithGoogle
{
    public sealed class LoginWithGoogleCommandHandler : IRequestHandler<LoginWithGoogleCommand, AuthResponseDto>
    {
        private const int ProfilePicturePlace = 0;

        private readonly IMediator _mediator;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly IGoogleAuth _googleAuth;
        private readonly IImageValidator _imageValidator;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IUserRefreshTokenRepository _refreshTokenRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly JwtSettings _jwtSettings;
        private readonly IStringLocalizer<Messages> _localizer;
        private readonly ILogger<LoginWithGoogleCommandHandler> _logger;

        public LoginWithGoogleCommandHandler(
            IMediator mediator,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole<Guid>> roleManager,
            IGoogleAuth googleAuth,
            IImageValidator imageValidator,
            IJwtTokenService jwtTokenService,
            IUserRefreshTokenRepository refreshTokenRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            IOptions<JwtSettings> jwtSettings,
            IStringLocalizer<Messages> localizer,
            ILogger<LoginWithGoogleCommandHandler> logger)
        {
            _mediator = mediator;
            _userManager = userManager;
            _roleManager = roleManager;
            _googleAuth = googleAuth;
            _imageValidator = imageValidator;
            _jwtTokenService = jwtTokenService;
            _refreshTokenRepository = refreshTokenRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _jwtSettings = jwtSettings.Value;
            _localizer = localizer;
            _logger = logger;
        }

        public async Task<AuthResponseDto> Handle(LoginWithGoogleCommand request, CancellationToken cancellationToken)
        {
            var correlationId = _currentUserService.CorrelationId;

            var payload = await _googleAuth.ValidateGoogleTokenAsync(request.IdToken, correlationId, cancellationToken);
            if (payload is null || string.IsNullOrWhiteSpace(payload.Subject))
                throw new UnAuthorizedException(_localizer[LocalizationKeys.AuthMessages.InvalidGoogleToken]);

            if (string.IsNullOrWhiteSpace(payload.Email))
                throw new BadRequestException(_localizer[LocalizationKeys.AuthMessages.GoogleEmailRequired]);

            if (!payload.EmailVerified)
                throw new UnAuthorizedException(_localizer[LocalizationKeys.AuthMessages.GoogleEmailNotVerified]);

            var email = payload.Email.Trim().ToLowerInvariant();

            var user = await _userManager.Users
                .FirstOrDefaultAsync(x => x.GoogleUserId == payload.Subject, cancellationToken);

            var isNewUser = false;
            if (user is null)
            {
                user = await _userManager.FindByEmailAsync(email);

                if (user is null)
                {
                    await EnsureEmailIsNotRetiredAsync(email, cancellationToken);
                    user = await CreateUserFromGoogleAsync(payload, email);
                    isNewUser = true;
                }
            }

            if (user.IsDeleted)
                throw new ForbiddenException(_localizer[LocalizationKeys.AuthMessages.AccountDeleted]);

            if (!user.IsActive)
                throw new ForbiddenException(_localizer[LocalizationKeys.AuthMessages.AccountDeactivated]);

            if (!string.IsNullOrEmpty(user.GoogleUserId)
                && !string.Equals(user.GoogleUserId, payload.Subject, StringComparison.Ordinal))
            {
                throw new ConflictException(_localizer[LocalizationKeys.AuthMessages.GoogleAccountAlreadyLinked]);
            }

            await _googleAuth.LinkGoogleAccountIfNeeded(user, payload, correlationId);
            await _googleAuth.UpdateUserInfoFromGoogle(user, payload, correlationId);

            await StoreGoogleProfilePictureAsync(user, payload, cancellationToken);

            if (isNewUser)
                await AssignDefaultRoleAsync(user);

            var roles = await _userManager.GetRolesAsync(user);
            var accessToken = _jwtTokenService.GenerateAccessToken(user, roles);
            var refreshToken = await ResolveRefreshTokenAsync(user, cancellationToken);

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

        private async Task StoreGoogleProfilePictureAsync(ApplicationUser user, GoogleJsonWebSignature.Payload payload, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(payload.Picture))
                return;

            if (!string.IsNullOrWhiteSpace(user.ProfilePictureUrl) && !IsRemoteUrl(user.ProfilePictureUrl))
                return;

            var formFile = await _imageValidator.ConvertImageToFormFile(payload.Picture, cancellationToken);
            if (formFile is null)
            {
                _logger.LogWarning(
                    "Could not download Google profile picture for user {UserId}. CorrelationId: {CorrelationId}",
                    user.Id, _currentUserService.CorrelationId);

                return;
            }

            var uploadResult = await _mediator.Send(
                new UploadFileCommand
                {
                    File = formFile,
                    MediaType = MediaType.Image,
                    Place = ProfilePicturePlace
                },
                cancellationToken);

            if (!uploadResult.IsSuccess || string.IsNullOrWhiteSpace(uploadResult.Data))
            {
                _logger.LogWarning(
                    "Google profile picture upload failed for user {UserId}: {Message}. CorrelationId: {CorrelationId}",
                    user.Id, uploadResult.Message, _currentUserService.CorrelationId);

                return;
            }

            user.UpdateProfilePicture(uploadResult.Data);

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                _logger.LogWarning(
                    "Failed to persist Google profile picture for user {UserId}: {Errors}. CorrelationId: {CorrelationId}",
                    user.Id, string.Join(", ", updateResult.Errors.Select(e => e.Code)), _currentUserService.CorrelationId);
            }
        }

        private static bool IsRemoteUrl(string value) =>
            Uri.TryCreate(value, UriKind.Absolute, out var uri)
            && (uri.Scheme == Uri.UriSchemeHttps || uri.Scheme == Uri.UriSchemeHttp);

        private async Task EnsureEmailIsNotRetiredAsync(string email, CancellationToken cancellationToken)        {
            var retiredUser = await _userManager.Users
                .IgnoreQueryFilters()
                .AnyAsync(x => x.Email == email && x.IsDeleted, cancellationToken);

            if (retiredUser)
                throw new ForbiddenException(_localizer[LocalizationKeys.AuthMessages.AccountDeleted]);
        }

        private async Task<ApplicationUser> CreateUserFromGoogleAsync(GoogleJsonWebSignature.Payload payload, string email)
        {
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email
            };

            user.UpdateFullName(string.IsNullOrWhiteSpace(payload.Name) ? email : payload.Name);

            user.ConfirmEmail();

            var createResult = await _userManager.CreateAsync(user);
            if (!createResult.Succeeded)
                throw new BadRequestException(ToErrors(createResult), _localizer[LocalizationKeys.AuthMessages.GoogleUserCreationFailed]);

            return user;
        }

        private async Task AssignDefaultRoleAsync(ApplicationUser user)
        {
            var customerRole = UserType.Customer.ToString();

            if (!await _roleManager.RoleExistsAsync(customerRole))
            {
                var roleResult = await _roleManager.CreateAsync(new IdentityRole<Guid>(customerRole));
                if (!roleResult.Succeeded)
                    throw new BadRequestException(ToErrors(roleResult), _localizer[LocalizationKeys.AuthMessages.GoogleUserCreationFailed]);
            }

            var addResult = await _userManager.AddToRoleAsync(user, customerRole);
            if (!addResult.Succeeded)
                throw new BadRequestException(ToErrors(addResult), _localizer[LocalizationKeys.AuthMessages.GoogleUserCreationFailed]);
        }

        private async Task<string> ResolveRefreshTokenAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            var existingToken = await _refreshTokenRepository
                .GetFirstAsync(x => x.UserId == user.Id && !x.IsRevoked && !x.IsDeleted && x.IsActive && x.ExpiryDate > DateTime.Now, cancellationToken);

            if (existingToken is not null)
                return existingToken.Token;

            var expiredTokens = await _refreshTokenRepository
                .GetAllAsync(x => x.UserId == user.Id && (x.IsRevoked || x.ExpiryDate <= DateTime.Now))
                .ToListAsync(cancellationToken);

            foreach (var token in expiredTokens)
                token.Revoke();

            var refreshToken = _jwtTokenService.GenerateRefreshToken(user);
            var userRefreshToken = UserRefreshToken.Create(
                user.Id,
                refreshToken,
                DateTime.Now.AddDays(_jwtSettings.RefreshTokenExpiryDays));

            await _refreshTokenRepository.AddAsync(userRefreshToken);
            await _unitOfWork.SaveChangesAsync();

            return refreshToken;
        }

        private static IDictionary<string, string[]> ToErrors(IdentityResult result) =>
            result.Errors
                .GroupBy(e => e.Code)
                .ToDictionary(g => g.Key, g => g.Select(e => e.Description).ToArray());
    }
}
