using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.VideoCategories.Commands.AddVideoCategory;
using Stock_Exchange.Application.Features.VideoCategories.Commands.DeleteVideoCategory;
using Stock_Exchange.Application.Features.VideoCategories.Commands.UpdateVideoCategory;
using Stock_Exchange.Application.Features.VideoCategories.DTOs;
using Stock_Exchange.Application.Features.VideoCategories.Queries.GetAllVideoCategories;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Enums;
using Stock_Exchange.Filters;
using Stock_Exchange.Routes.V1;

namespace Stock_Exchange.Controllers.V1;

[ApiVersion("1.0")]
[Route(ApiRoutes.VideoCategories.Base)]
[Route("api/v{version:apiVersion}/videos/categories")]
public class VideoCategoriesController : BaseController
{
    /// <summary>
    /// Retrieves all active video categories, optionally filtered by search term.
    /// </summary>
    /// <param name="query">Optional search and language filter parameters.</param>
    /// <returns>List of video categories.</returns>
    /// <response code="200">Video categories retrieved successfully.</response>
    [HttpGet]
    [Route(ApiRoutes.VideoCategories.GetAll)]
    [RoleAuthorize]
    [ProducesResponseType(typeof(ApiResponse<List<VideoCategoryDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] GetAllVideoCategoriesQuery query)
    {
        var result = await Mediator.Send(query);

        if (!result.IsSuccess)
            return FromResult(result);

        return OkResult(result.Data, LocalizationKeys.ActionResults.Ok);
    }

    /// <summary>
    /// Creates a new video category.
    /// </summary>
    /// <param name="command">The category payload with English and Arabic names.</param>
    /// <returns>Created category.</returns>
    [HttpPost]
    [Route(ApiRoutes.VideoCategories.Add)]
    [RoleAuthorize(UserType.Admin)]
    [ProducesResponseType(typeof(ApiResponse<VideoCategoryDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<VideoCategoryDto>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Add([FromBody] AddVideoCategoryCommand command)
    {
        var result = await Mediator.Send(command);

        if (!result.IsSuccess)
            return FromResult(result);

        return CreatedResult(result.Data, LocalizationKeys.ActionResults.Created);
    }

    /// <summary>
    /// Updates an existing video category.
    /// </summary>
    /// <param name="command">The category update payload.</param>
    /// <param name="id">Optional category id from route.</param>
    /// <returns>Updated category.</returns>
    [HttpPut]
    [Route(ApiRoutes.VideoCategories.Update)]
    [Route("{id:guid}")]
    [RoleAuthorize(UserType.Admin)]
    [ProducesResponseType(typeof(ApiResponse<VideoCategoryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<VideoCategoryDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<VideoCategoryDto>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromBody] UpdateVideoCategoryCommand command, [FromRoute] Guid? id = null)
    {
        if (id.HasValue && id.Value != Guid.Empty)
            command.Id = id.Value;

        var result = await Mediator.Send(command);

        if (!result.IsSuccess)
            return FromResult(result);

        return OkResult(result.Data, LocalizationKeys.ActionResults.Updated);
    }

    /// <summary>
    /// Soft deletes a video category.
    /// </summary>
    /// <param name="id">Category ID.</param>
    /// <returns>True if deleted.</returns>
    [HttpDelete]
    [Route(ApiRoutes.VideoCategories.Delete)]
    [RoleAuthorize(UserType.Admin)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await Mediator.Send(new DeleteVideoCategoryCommand(id));

        if (!result.IsSuccess)
            return FromResult(result);

        return OkResult(result.Data, LocalizationKeys.ActionResults.Deleted);
    }
}
