using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.Auth.Commands.ForgetPassword;
using Stock_Exchange.Application.Features.Auth.Commands.Login;
using Stock_Exchange.Application.Features.Auth.Commands.Register;
using Stock_Exchange.Application.Features.Auth.Commands.ResetPassword;
using Stock_Exchange.Application.Features.Auth.Commands.VerifyOtp;
using Stock_Exchange.Application.Features.Auth.DTOs;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Routes.V1;

namespace Stock_Exchange.Controllers.V1;

[ApiVersion("1.0")]
[Route(ApiRoutes.Authentication.Base)]
public class AuthenticationController : BaseController
{
    /// <summary>
    /// Authenticates a user with email and password and returns JWT access and refresh tokens.
    /// </summary>
    /// <param name="command">The login credentials.</param>
    /// <returns>The authenticated user information and authentication tokens.</returns>
    /// <response code="200">Login successful.</response>
    /// <response code="400">Validation error occurred.</response>
    /// <response code="401">Invalid email or password.</response>
    /// <response code="403">Account is deleted or deactivated.</response>
    [HttpPost]
    [Route(ApiRoutes.Authentication.Login)]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult> Login([FromBody] LoginCommand command)
    {
        var result = await Mediator.Send(command);
        return OkResult(result, LocalizationKeys.ActionResults.Ok);
    }

    /// <summary>
    /// Registers a new user account and returns JWT access and refresh tokens.
    /// </summary>
    /// <param name="command">The registration details.</param>
    /// <returns>The created user information and authentication tokens.</returns>
    /// <response code="201">User registered successfully.</response>
    /// <response code="400">Validation error occurred.</response>
    /// <response code="409">Email or phone number already in use.</response>
    [HttpPost]
    [Route(ApiRoutes.Authentication.Register)]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status409Conflict)]
    public async Task<ActionResult> Register([FromBody] SignupCommand command)
    {
        var result = await Mediator.Send(command);
        return CreatedResult(result, LocalizationKeys.ActionResults.Created);
    }

    /// <summary>
    /// Sends a password reset verification code to the specified email address.
    /// </summary>
    /// <param name="command">The email address of the account to recover.</param>
    /// <returns>Confirmation that the verification code was sent.</returns>
    /// <response code="200">Verification code sent successfully.</response>
    /// <response code="400">Validation error occurred.</response>
    /// <response code="404">User not found.</response>
    [HttpPost]
    [Route(ApiRoutes.Authentication.ForgetPassword)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> ForgetPassword([FromBody] ForgetPasswordCommand command)
    {
        var result = await Mediator.Send(command);
        return OkResult(result, LocalizationKeys.ActionResults.Ok);
    }

    /// <summary>
    /// Verifies the OTP verification code sent to the specified email address and generates a password reset token.
    /// </summary>
    /// <param name="command">The email address and OTP verification code.</param>
    /// <returns>A password reset token to authorize password reset.</returns>
    /// <response code="200">Verification code verified successfully and reset token generated.</response>
    /// <response code="400">Validation error occurred, code is invalid, or code has expired.</response>
    /// <response code="404">User not found.</response>
    [HttpPost]
    [Route(ApiRoutes.Authentication.VerifyOtp)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> VerifyOtp([FromBody] VerifyOtpCommand command)
    {
        var result = await Mediator.Send(command);
        return OkResult(result, LocalizationKeys.ActionResults.Ok);
    }

    /// <summary>
    /// Resets the user's password using the token received from OTP verification.
    /// </summary>
    /// <param name="command">The email address, reset token, and new password.</param>
    /// <returns>Confirmation that the password was reset successfully.</returns>
    /// <response code="200">Password reset successfully.</response>
    /// <response code="400">Validation error occurred, token is invalid or expired, or passwords do not match.</response>
    /// <response code="404">User not found.</response>
    [HttpPost]
    [Route(ApiRoutes.Authentication.ResetPassword)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordCommand command)
    {
        var result = await Mediator.Send(command);
        return OkResult(result, LocalizationKeys.AuthMessages.PasswordResetSuccess);
    }
}
