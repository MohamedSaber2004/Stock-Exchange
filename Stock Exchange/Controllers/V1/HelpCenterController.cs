using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.HelpCenter.Commands.AddHelpCenter;
using Stock_Exchange.Application.Features.HelpCenter.Commands.DeleteHelpCenter;
using Stock_Exchange.Application.Features.HelpCenter.Commands.UpdateHelpCenter;
using Stock_Exchange.Application.Features.HelpCenter.DTOs;
using Stock_Exchange.Application.Features.HelpCenter.Queries.GetAllHelpCenters;
using Stock_Exchange.Application.Features.HelpCenter.Queries.GetHelpCenterById;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Enums;
using Stock_Exchange.Filters;
using Stock_Exchange.Routes.V1;

namespace Stock_Exchange.Controllers.V1;

[ApiVersion("1.0")]
[Route(ApiRoutes.HelpCenter.Base)]
public class HelpCenterController : BaseController
{
    /// <summary>
    /// Retrieves all active help center entries, optionally filtered by category or search term.
    /// The response language is taken from the request Accept-Language header.
    /// </summary>
    /// <param name="query">Optional category filter and search term.</param>
    /// <returns>The matching help center entries.</returns>
    /// <response code="200">Help center entries retrieved successfully.</response>
    [HttpGet]
    [Route(ApiRoutes.HelpCenter.GetAll)]
    [RoleAuthorize]
    [ProducesResponseType(typeof(ApiResponse<List<HelpCenterDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] GetAllHelpCentersQuery query)
    {
        var result = await Mediator.Send(query);

        if (!result.IsSuccess)
            return FromResult(result);

        return OkResult(result.Data, LocalizationKeys.ActionResults.Ok);
    }

    /// <summary>
    /// Retrieves a single help center entry by its id.
    /// The response language is taken from the request Accept-Language header.
    /// </summary>
    /// <param name="id">The help center entry id.</param>
    /// <returns>The help center entry in the requested language.</returns>
    /// <response code="200">Help center entry retrieved successfully.</response>
    /// <response code="404">The help center entry was not found.</response>
    [HttpGet]
    [Route(ApiRoutes.HelpCenter.GetById)]
    [RoleAuthorize]
    [ProducesResponseType(typeof(ApiResponse<HelpCenterDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<HelpCenterDto>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await Mediator.Send(new GetHelpCenterByIdQuery(id));

        if (!result.IsSuccess)
            return FromResult(result);

        return OkResult(result.Data, LocalizationKeys.ActionResults.Ok);
    }

    /// <summary>
    /// Creates a new help center entry.
    /// </summary>
    /// <param name="command">The help center entry to create.</param>
    /// <returns>The created help center entry.</returns>
    /// <response code="201">Help center entry created successfully.</response>
    /// <response code="400">One or more fields are missing or exceed the allowed length.</response>
    [HttpPost]
    [Route(ApiRoutes.HelpCenter.Add)]
    [RoleAuthorize(UserType.Admin)]
    [ProducesResponseType(typeof(ApiResponse<HelpCenterDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<HelpCenterDto>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Add([FromBody] AddHelpCenterCommand command)
    {
        var result = await Mediator.Send(command);

        if (!result.IsSuccess)
            return FromResult(result);

        return CreatedResult(result.Data, LocalizationKeys.ActionResults.Created);
    }

    /// <summary>
    /// Updates an existing help center entry.
    /// </summary>
    /// <param name="command">The help center entry id and its new values.</param>
    /// <returns>The updated help center entry.</returns>
    /// <response code="200">Help center entry updated successfully.</response>
    /// <response code="400">One or more fields are missing or exceed the allowed length.</response>
    /// <response code="404">The help center entry was not found.</response>
    [HttpPut]
    [Route(ApiRoutes.HelpCenter.Update)]
    [RoleAuthorize(UserType.Admin)]
    [ProducesResponseType(typeof(ApiResponse<HelpCenterDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<HelpCenterDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<HelpCenterDto>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromBody] UpdateHelpCenterCommand command)
    {
        var result = await Mediator.Send(command);

        if (!result.IsSuccess)
            return FromResult(result);

        return OkResult(result.Data, LocalizationKeys.ActionResults.Updated);
    }

    /// <summary>
    /// Soft deletes a help center entry.
    /// </summary>
    /// <param name="id">The help center entry id.</param>
    /// <returns>Whether the entry was deleted.</returns>
    /// <response code="200">Help center entry deleted successfully.</response>
    /// <response code="404">The help center entry was not found.</response>
    [HttpDelete]
    [Route(ApiRoutes.HelpCenter.Delete)]
    [RoleAuthorize(UserType.Admin)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await Mediator.Send(new DeleteHelpCenterCommand(id));

        if (!result.IsSuccess)
            return FromResult(result);

        return OkResult(result.Data, LocalizationKeys.ActionResults.Deleted);
    }
}
