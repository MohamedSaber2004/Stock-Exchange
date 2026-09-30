using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.TermsAndConditions.Commands.DeleteTermsAndConditions;
using Stock_Exchange.Application.Features.TermsAndConditions.Commands.UpdateTermsAndConditions;
using Stock_Exchange.Application.Features.TermsAndConditions.DTOs;
using Stock_Exchange.Application.Features.TermsAndConditions.Queries.GetTermsAndConditions;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Enums;
using Stock_Exchange.Filters;
using Stock_Exchange.Routes.V1;

namespace Stock_Exchange.Controllers.V1;

[ApiVersion("1.0")]
[Route(ApiRoutes.TermsAndConditions.Base)]
public class TermsAndConditionsController : BaseController
{
    /// <summary>
    /// Retrieves the terms and conditions content and its sections.
    /// The response language is taken from the request Accept-Language header.
    /// </summary>
    /// <returns>The terms and conditions in the requested language.</returns>
    /// <response code="200">Terms and conditions retrieved successfully.</response>
    /// <response code="404">Terms and conditions was not found.</response>
    [HttpGet]
    [Route(ApiRoutes.TermsAndConditions.Get)]
    [RoleAuthorize]
    [ProducesResponseType(typeof(ApiResponse<TermsAndConditionsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<TermsAndConditionsDto>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get()
    {
        var result = await Mediator.Send(new GetTermsAndConditionsQuery());

        if (!result.IsSuccess)
            return FromResult(result);

        return OkResult(result.Data, LocalizationKeys.ActionResults.Ok);
    }

    /// <summary>
    /// Creates or updates the terms and conditions content and its sections.
    /// </summary>
    /// <param name="command">The terms and conditions titles and sections to update.</param>
    /// <returns>The saved terms and conditions content.</returns>
    /// <response code="200">Terms and conditions saved successfully.</response>
    /// <response code="400">One or more fields exceed the allowed length or are invalid.</response>
    /// <response code="401">The caller is not authenticated.</response>
    /// <response code="403">The caller is not authorized.</response>
    [HttpPatch]
    [Route(ApiRoutes.TermsAndConditions.Update)]
    [RoleAuthorize(UserType.Admin)]
    [ProducesResponseType(typeof(ApiResponse<TermsAndConditionsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<TermsAndConditionsDto>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update([FromBody] UpdateTermsAndConditionsCommand command)
    {
        var result = await Mediator.Send(command);

        if (!result.IsSuccess)
            return FromResult(result);

        return OkResult(result.Data, LocalizationKeys.ActionResults.Updated);
    }

    /// <summary>
    /// Soft deletes the active terms and conditions.
    /// </summary>
    /// <returns>Whether the terms and conditions was deleted.</returns>
    /// <response code="200">Terms and conditions deleted successfully.</response>
    /// <response code="404">Terms and conditions was not found.</response>
    [HttpDelete]
    [Route(ApiRoutes.TermsAndConditions.Delete)]
    [RoleAuthorize(UserType.Admin)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete()
    {
        var result = await Mediator.Send(new DeleteTermsAndConditionsCommand());

        if (!result.IsSuccess)
            return FromResult(result);

        return OkResult(result.Data, LocalizationKeys.ActionResults.Deleted);
    }

    /// <summary>
    /// Soft deletes a specific terms and conditions section or the terms and conditions by id.
    /// </summary>
    /// <param name="id">The id of the section or policy to delete.</param>
    /// <returns>Whether the entity was deleted.</returns>
    /// <response code="200">Entry deleted successfully.</response>
    /// <response code="404">Entry was not found.</response>
    [HttpDelete]
    [Route(ApiRoutes.TermsAndConditions.DeleteById)]
    [RoleAuthorize(UserType.Admin)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteById(Guid id)
    {
        var result = await Mediator.Send(new DeleteTermsAndConditionsCommand(id));

        if (!result.IsSuccess)
            return FromResult(result);

        return OkResult(result.Data, LocalizationKeys.ActionResults.Deleted);
    }
}
