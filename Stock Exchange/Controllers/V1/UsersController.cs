using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.Users.Commands.AddUser;
using Stock_Exchange.Application.Features.Users.Commands.AdminChangePassword;
using Stock_Exchange.Application.Features.Users.Commands.DeleteUser;
using Stock_Exchange.Application.Features.Users.Commands.UpdateUser;
using Stock_Exchange.Application.Features.Users.DTOs;
using Stock_Exchange.Application.Features.Users.Queries.GetAllUsers;
using Stock_Exchange.Application.Features.Users.Queries.GetUserById;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Enums;
using Stock_Exchange.Filters;
using Stock_Exchange.Routes.V1;

namespace Stock_Exchange.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route(ApiRoutes.Users.Base)]
    [RoleAuthorize(UserType.Admin)]
    public class UsersController : BaseController
    {
        /// <summary>
        /// Retrieves paginated list of users with search and filter capabilities for admin dashboard.
        /// </summary>
        /// <param name="query">Search term, userType filter (Admin/Customer), isActive filter, and pagination parameters.</param>
        /// <returns>Paginated users list.</returns>
        [HttpGet]
        [Route(ApiRoutes.Users.GetAll)]
        [ProducesResponseType(typeof(ApiResponse<PagginatedResult<UserDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] GetAllUsersQuery query)
        {
            var result = await Mediator.Send(query);
            if (!result.IsSuccess)
                return FromResult(result);

            return OkResult(result.Data, LocalizationKeys.ActionResults.Ok);
        }

        /// <summary>
        /// Retrieves user details by user ID.
        /// </summary>
        /// <param name="id">The user ID.</param>
        /// <returns>User details.</returns>
        [HttpGet]
        [Route(ApiRoutes.Users.GetById)]
        [ProducesResponseType(typeof(ApiResponse<UserDetailsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await Mediator.Send(new GetUserByIdQuery(id));
            if (!result.IsSuccess)
                return FromResult(result);

            return OkResult(result.Data, LocalizationKeys.ActionResults.Ok);
        }

        /// <summary>
        /// Creates a new user account with specified role (Admin or Customer).
        /// </summary>
        /// <param name="command">User registration details.</param>
        /// <returns>The created user.</returns>
        [HttpPost]
        [Route(ApiRoutes.Users.Add)]
        [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Add([FromBody] AddUserCommand command)
        {
            var result = await Mediator.Send(command);
            if (!result.IsSuccess)
                return FromResult(result);

            return CreatedResult(result.Data, LocalizationKeys.ActionResults.Created);
        }

        /// <summary>
        /// Updates an existing user's information and role/status.
        /// </summary>
        /// <param name="id">The user ID.</param>
        /// <param name="command">Updated user information.</param>
        /// <returns>The updated user.</returns>
        [HttpPut]
        [Route(ApiRoutes.Users.Update)]
        [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserCommand command)
        {
            var targetCommand = command with { Id = id };
            var result = await Mediator.Send(targetCommand);
            if (!result.IsSuccess)
                return FromResult(result);

            return OkResult(result.Data, LocalizationKeys.ActionResults.Updated);
        }

        /// <summary>
        /// Soft deletes a user account and revokes their active tokens.
        /// </summary>
        /// <param name="id">The user ID.</param>
        /// <returns>Action result.</returns>
        [HttpDelete]
        [Route(ApiRoutes.Users.Delete)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await Mediator.Send(new DeleteUserCommand(id));
            if (!result.IsSuccess)
                return FromResult(result);

            return OkResult(result.Data, LocalizationKeys.ActionResults.Deleted);
        }

        /// <summary>
        /// Resets/changes a user's password directly by admin.
        /// </summary>
        /// <param name="id">The user ID.</param>
        /// <param name="request">New password and confirmation.</param>
        /// <returns>Action result.</returns>
        [HttpPut]
        [Route(ApiRoutes.Users.ChangePassword)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ChangePassword(Guid id, [FromBody] AdminChangePasswordRequest request)
        {
            var command = new AdminChangePasswordCommand(id, request.NewPassword, request.ConfirmNewPassword);
            var result = await Mediator.Send(command);
            if (!result.IsSuccess)
                return FromResult(result);

            return OkResult(result.Data, LocalizationKeys.ActionResults.Updated);
        }
    }

    public record AdminChangePasswordRequest
    {
        public string NewPassword { get; init; } = null!;
        public string ConfirmNewPassword { get; init; } = null!;
    }
}
