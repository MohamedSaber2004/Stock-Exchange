using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.PrivacyPolicy.Commands.DeletePrivacy;
using Stock_Exchange.Application.Features.PrivacyPolicy.Commands.UpdatePrivacy;
using Stock_Exchange.Application.Features.PrivacyPolicy.DTOs;
using Stock_Exchange.Application.Features.PrivacyPolicy.Queries.GetPrivacy;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Enums;
using Stock_Exchange.Filters;
using Stock_Exchange.Routes.V1;

namespace Stock_Exchange.Controllers.V1;

[ApiVersion("1.0")]
[Route(ApiRoutes.PrivacyPolicy.Base)]
public class PrivacyPolicyController : BaseController
{
    /// <summary>
    /// Retrieves the privacy policy content and its sections.
    /// The response language is taken from the request Accept-Language header.
    /// </summary>
    /// <returns>The privacy policy in the requested language.</returns>
    /// <response code="200">Privacy policy retrieved successfully.</response>
    /// <response code="404">Privacy policy was not found.</response>
    [HttpGet]
    [Route(ApiRoutes.PrivacyPolicy.Get)]
    [RoleAuthorize]
    [ProducesResponseType(typeof(ApiResponse<PrivacyPolicyDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<PrivacyPolicyDto>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get()
    {
        var result = await Mediator.Send(new GetPrivacyQuery());

        if (!result.IsSuccess)
            return FromResult(result);

        return OkResult(result.Data, LocalizationKeys.ActionResults.Ok);
    }

    /// <summary>
    /// Creates or updates the privacy policy content and its sections.
    /// </summary>
    /// <param name="command">The privacy policy titles and sections to update.</param>
    /// <returns>The saved privacy policy content.</returns>
    /// <response code="200">Privacy policy saved successfully.</response>
    /// <response code="400">One or more fields exceed the allowed length or are invalid.</response>
    /// <response code="401">The caller is not authenticated.</response>
    /// <response code="403">The caller is not authorized.</response>
    [HttpPut]
    [HttpPatch]
    [Route(ApiRoutes.PrivacyPolicy.Update)]
    [RoleAuthorize(UserType.Admin)]
    [ProducesResponseType(typeof(ApiResponse<PrivacyPolicyDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<PrivacyPolicyDto>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update([FromBody] UpdatePrivacyCommand command)
    {
        var result = await Mediator.Send(command);

        if (!result.IsSuccess)
            return FromResult(result);

        return OkResult(result.Data, LocalizationKeys.ActionResults.Updated);
    }

    /// <summary>
    /// Soft deletes the active privacy policy.
    /// </summary>
    /// <returns>Whether the privacy policy was deleted.</returns>
    /// <response code="200">Privacy policy deleted successfully.</response>
    /// <response code="404">Privacy policy was not found.</response>
    [HttpDelete]
    [Route(ApiRoutes.PrivacyPolicy.Delete)]
    [RoleAuthorize(UserType.Admin)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete()
    {
        var result = await Mediator.Send(new DeletePrivacyCommand());

        if (!result.IsSuccess)
            return FromResult(result);

        return OkResult(result.Data, LocalizationKeys.ActionResults.Deleted);
    }

    /// <summary>
    /// Soft deletes a specific privacy policy section or the privacy policy by id.
    /// </summary>
    /// <param name="id">The id of the section or policy to delete.</param>
    /// <returns>Whether the entity was deleted.</returns>
    /// <response code="200">Entry deleted successfully.</response>
    /// <response code="404">Entry was not found.</response>
    [HttpDelete]
    [Route(ApiRoutes.PrivacyPolicy.DeleteById)]
    [RoleAuthorize(UserType.Admin)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteById(Guid id)
    {
        var result = await Mediator.Send(new DeletePrivacyCommand(id));

        if (!result.IsSuccess)
            return FromResult(result);

        return OkResult(result.Data, LocalizationKeys.ActionResults.Deleted);
    }
}
