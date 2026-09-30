using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.HelpCenterCategories.Commands.AddHelpCenterCategory;
using Stock_Exchange.Application.Features.HelpCenterCategories.Commands.DeleteHelpCenterCategory;
using Stock_Exchange.Application.Features.HelpCenterCategories.Commands.UpdateHelpCenterCategory;
using Stock_Exchange.Application.Features.HelpCenterCategories.DTOs;
using Stock_Exchange.Application.Features.HelpCenterCategories.Queries.GetAllHelpCenterCategories;
using Stock_Exchange.Application.Features.HelpCenterCategories.Queries.GetAllHelpCenterCategoryById;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Enums;
using Stock_Exchange.Filters;
using Stock_Exchange.Routes.V1;

namespace Stock_Exchange.Controllers.V1;

[ApiVersion("1.0")]
[Route(ApiRoutes.HelpCenterCategories.Base)]
public class HelpCenterCategoriesController : BaseController
{
    /// <summary>
    /// Retrieves all active help center categories, optionally filtered by search term.
    /// The response language is taken from the request Accept-Language header.
    /// </summary>
    /// <param name="query">Optional search term.</param>
    /// <returns>The matching help center categories.</returns>
    /// <response code="200">Help center categories retrieved successfully.</response>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<List<HelpCenterCategoryDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetAll([FromQuery] GetAllHelpCenterCategoriesQuery query)
    {
        var result = await Mediator.Send(query);

        if (!result.IsSuccess)
            return FromResult(result);

        return OkResult(result.Data, LocalizationKeys.ActionResults.Ok);
    }

    /// <summary>
    /// Retrieves a single help center category by its id.
    /// The response language is taken from the request Accept-Language header.
    /// </summary>
    /// <param name="id">The help center category id.</param>
    /// <returns>The help center category in the requested language.</returns>
    /// <response code="200">Help center category retrieved successfully.</response>
    /// <response code="404">The help center category was not found.</response>
    [HttpGet]
    [Route(ApiRoutes.HelpCenterCategories.GetById)]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<HelpCenterCategoryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<HelpCenterCategoryDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetById(Guid id)
    {
        var result = await Mediator.Send(new GetHelpCenterCategoryByIdQuery(id));

        if (!result.IsSuccess)
            return FromResult(result);

        return OkResult(result.Data, LocalizationKeys.ActionResults.Ok);
    }

    /// <summary>
    /// Creates a new help center category.
    /// </summary>
    /// <param name="command">The help center category to create.</param>
    /// <returns>The created help center category.</returns>
    /// <response code="201">Help center category created successfully.</response>
    /// <response code="400">One or more fields are missing or exceed the allowed length.</response>
    [HttpPost]
    [RoleAuthorize(UserType.Admin)]
    [ProducesResponseType(typeof(ApiResponse<HelpCenterCategoryDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<HelpCenterCategoryDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Add([FromBody] AddHelpCenterCategoryCommand command)
    {
        var result = await Mediator.Send(command);

        if (!result.IsSuccess)
            return FromResult(result);

        return CreatedResult(result.Data, LocalizationKeys.ActionResults.Created);
    }

    /// <summary>
    /// Updates an existing help center category.
    /// </summary>
    /// <param name="command">The help center category id and its new values.</param>
    /// <returns>The updated help center category.</returns>
    /// <response code="200">Help center category updated successfully.</response>
    /// <response code="400">One or more fields are missing or exceed the allowed length.</response>
    /// <response code="404">The help center category was not found.</response>
    [HttpPut]
    [RoleAuthorize(UserType.Admin)]
    [ProducesResponseType(typeof(ApiResponse<HelpCenterCategoryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<HelpCenterCategoryDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<HelpCenterCategoryDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Update([FromBody] UpdateHelpCenterCategoryCommand command)
    {
        var result = await Mediator.Send(command);

        if (!result.IsSuccess)
            return FromResult(result);

        return OkResult(result.Data, LocalizationKeys.ActionResults.Updated);
    }

    /// <summary>
    /// Soft deletes a help center category.
    /// </summary>
    /// <param name="id">The help center category id.</param>
    /// <returns>Whether the category was deleted.</returns>
    /// <response code="200">Help center category deleted successfully.</response>
    /// <response code="404">The help center category was not found.</response>
    [HttpDelete]
    [Route(ApiRoutes.HelpCenterCategories.GetById)]
    [RoleAuthorize(UserType.Admin)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(Guid id)
    {
        var result = await Mediator.Send(new DeleteHelpCenterCategoryCommand(id));

        if (!result.IsSuccess)
            return FromResult(result);

        return OkResult(result.Data, LocalizationKeys.ActionResults.Deleted);
    }
}
