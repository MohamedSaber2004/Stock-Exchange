using FluentValidation.Resources;

namespace Stock_Exchange.Application.Features.Auth.DTOs
{
    public record AuthResponseDto(
        string? AccessToken,
        string? RefreshToken,
        string FullName,
        string Email,
        string PhoneNumber,
        string Roles,
        Guid id,
        string? ProfilePictureUrl);

    public record RefreshTokenResponseDto(
        string AccessToken,
        string RefreshToken);


    public record UserProfileDto(
        Guid Id,
        string FullName,
        string Email,
        string PhoneNumber,
        string? ProfilePictureUrl,
        Language Language);
}
