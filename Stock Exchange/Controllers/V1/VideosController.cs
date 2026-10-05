using Microsoft.AspNetCore.Authorization;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.Videos.Commands.AddVideo;
using Stock_Exchange.Application.Features.Videos.Commands.DeleteVideo;
using Stock_Exchange.Application.Features.Videos.Commands.UpdateVideo;
using Stock_Exchange.Application.Features.Videos.DTOs;
using Stock_Exchange.Application.Features.Videos.Queries.GetAllVideos;
using Stock_Exchange.Application.Features.Videos.Queries.GetVideoById;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Enums;
using Stock_Exchange.Filters;
using Stock_Exchange.Routes.V1;

namespace Stock_Exchange.Controllers.V1;

[ApiVersion("1.0")]
[Route(ApiRoutes.Videos.Base)]
public class VideosController : BaseController
{
    /// <summary>
    /// Retrieves paginated videos (default page size: 10), with optional search, category, and active status filters.
    /// General authorization: Accessible to any authenticated user.
    /// </summary>
    /// <param name="query">Pagination parameters, search term, category filter, and status.</param>
    /// <returns>A paginated list of videos.</returns>
    /// <response code="200">Videos retrieved successfully.</response>
    [HttpGet]
    [Route(ApiRoutes.Videos.GetAll)]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<PagginatedResult<VideoDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] GetAllVideosQuery query)
    {
        var result = await Mediator.Send(query);

        if (!result.IsSuccess)
            return FromResult(result);

        return OkResult(result.Data, LocalizationKeys.ActionResults.Ok);
    }

    /// <summary>
    /// Retrieves a single video by its id.
    /// General authorization: Accessible to any authenticated user.
    /// </summary>
    /// <param name="id">The video id.</param>
    /// <param name="applyLanguageFilter">Optional flag to apply language filter based on Accept-Language header (default true).</param>
    /// <returns>The video data.</returns>
    /// <response code="200">Video retrieved successfully.</response>
    /// <response code="404">Video was not found.</response>
    [HttpGet]
    [Route(ApiRoutes.Videos.GetById)]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<VideoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<VideoDto>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, [FromQuery] bool? applyLanguageFilter = null)
    {
        var result = await Mediator.Send(new GetVideoByIdQuery(id, applyLanguageFilter));

        if (!result.IsSuccess)
            return FromResult(result);

        return OkResult(result.Data, LocalizationKeys.ActionResults.Ok);
    }

    /// <summary>
    /// Creates a new video.
    /// Admin authorization only.
    /// </summary>
    /// <param name="command">The video data to create.</param>
    /// <returns>The created video.</returns>
    /// <response code="201">Video created successfully.</response>
    /// <response code="400">One or more fields are invalid or exceed allowed lengths.</response>
    /// <response code="401">User is unauthorized.</response>
    /// <response code="403">User is forbidden (admin only).</response>
    [HttpPost]
    [Route(ApiRoutes.Videos.Add)]
    [RoleAuthorize(UserType.Admin)]
    [ProducesResponseType(typeof(ApiResponse<VideoDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<VideoDto>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Add([FromBody] AddVideoCommand command)
    {
        var result = await Mediator.Send(command);

        if (!result.IsSuccess)
            return FromResult(result);

        return CreatedResult(result.Data, LocalizationKeys.ActionResults.Created);
    }

    /// <summary>
    /// Updates an existing video by route id.
    /// Admin authorization only.
    /// </summary>
    /// <param name="id">The video id from route.</param>
    /// <param name="command">The updated properties.</param>
    /// <returns>The updated video.</returns>
    /// <response code="200">Video updated successfully.</response>
    /// <response code="400">One or more fields are invalid or exceed allowed lengths.</response>
    /// <response code="401">User is unauthorized.</response>
    /// <response code="403">User is forbidden (admin only).</response>
    /// <response code="404">Video was not found.</response>
    [HttpPut]
    [Route(ApiRoutes.Videos.Update)]
    [RoleAuthorize(UserType.Admin)]
    [ProducesResponseType(typeof(ApiResponse<VideoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<VideoDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<VideoDto>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateVideoCommand command)
    {
        command.Id = id;
        var result = await Mediator.Send(command);

        if (!result.IsSuccess)
            return FromResult(result);

        return OkResult(result.Data, LocalizationKeys.ActionResults.Updated);
    }

    /// <summary>
    /// Hard deletes a video permanently from the database.
    /// Admin authorization only.
    /// </summary>
    /// <param name="id">The video id to permanently delete.</param>
    /// <returns>True if successfully hard deleted.</returns>
    /// <response code="200">Video permanently deleted successfully.</response>
    /// <response code="401">User is unauthorized.</response>
    /// <response code="403">User is forbidden (admin only).</response>
    /// <response code="404">Video was not found.</response>
    [HttpDelete]
    [Route(ApiRoutes.Videos.Delete)]
    [RoleAuthorize(UserType.Admin)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await Mediator.Send(new DeleteVideoCommand(id));

        if (!result.IsSuccess)
            return FromResult(result);

        return OkResult(result.Data, LocalizationKeys.ActionResults.Deleted);
    }
}
