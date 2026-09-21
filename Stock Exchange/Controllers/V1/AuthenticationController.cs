using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.Auth.Commands.Login;
using Stock_Exchange.Application.Features.Auth.Commands.Register;
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
}
