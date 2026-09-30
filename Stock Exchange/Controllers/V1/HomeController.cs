using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.Home.Commands.UpdateHome;
using Stock_Exchange.Application.Features.Home.Commands.UpdateHomeArticles;
using Stock_Exchange.Application.Features.Home.Commands.UpdateHomeExperts;
using Stock_Exchange.Application.Features.Home.Commands.UpdateHomeNews;
using Stock_Exchange.Application.Features.Home.Commands.UpdateHomePlans;
using Stock_Exchange.Application.Features.Home.Commands.UpdateHomeServices;
using Stock_Exchange.Application.Features.Home.Commands.UpdateHomeVideos;
using Stock_Exchange.Application.Features.Home.DTOs;
using Stock_Exchange.Application.Features.Home.Queries.GetHome;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Enums;
using Stock_Exchange.Filters;
using Stock_Exchange.Routes.V1;

namespace Stock_Exchange.Controllers.V1;

[ApiVersion("1.0")]
[Route(ApiRoutes.Home.Base)]
public class HomeController : BaseController
{
    /// <summary>
    /// Retrieves the aggregated home feed for the authenticated user, including greeting name,
    /// hero banner, latest news, services, articles, featured videos, subscription plans, and experts.
    /// The response language is determined by the Accept-Language request header.
    /// </summary>
    /// <returns>The aggregated home feed in the requested language.</returns>
    /// <response code="200">Home feed retrieved successfully.</response>
    /// <response code="401">The caller is not authenticated.</response>
    [HttpGet]
    [Route(ApiRoutes.Home.Get)]
    [RoleAuthorize]
    [ProducesResponseType(typeof(ApiResponse<HomeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Get()
    {
        var result = await Mediator.Send(new GetHomeQuery());

        if (!result.IsSuccess)
            return FromResult(result);

        return OkResult(result.Data, LocalizationKeys.ActionResults.Ok);
    }

    /// <summary>
    /// Partially updates the Home hero section (admin only).
    /// Fields provided as null will remain unchanged.
    /// </summary>
    /// <param name="command">The hero fields to update.</param>
    /// <returns>The updated hero section.</returns>
    /// <response code="200">Home hero section updated successfully.</response>
    /// <response code="400">One or more fields exceed the allowed length or are invalid.</response>
    /// <response code="401">The caller is not authenticated.</response>
    /// <response code="403">The caller is not authorized as Admin.</response>
    [HttpPatch]
    [Route(ApiRoutes.Home.UpdateHero)]
    [Route(ApiRoutes.Home.Update)]
    [RoleAuthorize(UserType.Admin)]
    [ProducesResponseType(typeof(ApiResponse<HomeHeroDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<HomeHeroDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateHero([FromBody] UpdateHomeCommand command)
    {
        var result = await Mediator.Send(command);

        if (!result.IsSuccess)
            return FromResult(result);

        return OkResult(result.Data, LocalizationKeys.ActionResults.Updated);
    }

    /// <summary>
    /// Updates the list of latest news items displayed on the Home feed (admin only).
    /// Syncs existing, new, and removed news items.
    /// </summary>
    /// <param name="command">The list of news items to update or create.</param>
    /// <returns>The updated list of latest news items.</returns>
    /// <response code="200">News items updated successfully.</response>
    /// <response code="400">One or more fields are invalid.</response>
    /// <response code="401">The caller is not authenticated.</response>
    /// <response code="403">The caller is not authorized as Admin.</response>
    [HttpPut]
    [Route(ApiRoutes.Home.UpdateNews)]
    [RoleAuthorize(UserType.Admin)]
    [ProducesResponseType(typeof(ApiResponse<List<HomeNewsDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<List<HomeNewsDto>>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateNews([FromBody] UpdateHomeNewsCommand command)
    {
        var result = await Mediator.Send(command);

        if (!result.IsSuccess)
            return FromResult(result);

        return OkResult(result.Data, LocalizationKeys.ActionResults.Updated);
    }

    /// <summary>
    /// Updates the list of services displayed on the Home feed (admin only).
    /// Syncs existing, new, and removed services.
    /// </summary>
    /// <param name="command">The list of services to update or create.</param>
    /// <returns>The updated list of services.</returns>
    /// <response code="200">Services updated successfully.</response>
    /// <response code="400">One or more fields are invalid.</response>
    /// <response code="401">The caller is not authenticated.</response>
    /// <response code="403">The caller is not authorized as Admin.</response>
    [HttpPut]
    [Route(ApiRoutes.Home.UpdateServices)]
    [RoleAuthorize(UserType.Admin)]
    [ProducesResponseType(typeof(ApiResponse<List<HomeServiceDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<List<HomeServiceDto>>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateServices([FromBody] UpdateHomeServicesCommand command)
    {
        var result = await Mediator.Send(command);

        if (!result.IsSuccess)
            return FromResult(result);

        return OkResult(result.Data, LocalizationKeys.ActionResults.Updated);
    }

    /// <summary>
    /// Updates the list of articles displayed on the Home feed (admin only).
    /// Syncs existing, new, and removed articles.
    /// </summary>
    /// <param name="command">The list of articles to update or create.</param>
    /// <returns>The updated list of articles.</returns>
    /// <response code="200">Articles updated successfully.</response>
    /// <response code="400">One or more fields are invalid.</response>
    /// <response code="401">The caller is not authenticated.</response>
    /// <response code="403">The caller is not authorized as Admin.</response>
    [HttpPut]
    [Route(ApiRoutes.Home.UpdateArticles)]
    [RoleAuthorize(UserType.Admin)]
    [ProducesResponseType(typeof(ApiResponse<List<HomeArticleDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<List<HomeArticleDto>>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateArticles([FromBody] UpdateHomeArticlesCommand command)
    {
        var result = await Mediator.Send(command);

        if (!result.IsSuccess)
            return FromResult(result);

        return OkResult(result.Data, LocalizationKeys.ActionResults.Updated);
    }

    /// <summary>
    /// Updates the list of videos displayed on the Home feed (admin only).
    /// Syncs existing, new, and removed videos.
    /// </summary>
    /// <param name="command">The list of videos to update or create.</param>
    /// <returns>The updated list of videos.</returns>
    /// <response code="200">Videos updated successfully.</response>
    /// <response code="400">One or more fields are invalid.</response>
    /// <response code="401">The caller is not authenticated.</response>
    /// <response code="403">The caller is not authorized as Admin.</response>
    [HttpPut]
    [Route(ApiRoutes.Home.UpdateVideos)]
    [RoleAuthorize(UserType.Admin)]
    [ProducesResponseType(typeof(ApiResponse<List<HomeVideoDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<List<HomeVideoDto>>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateVideos([FromBody] UpdateHomeVideosCommand command)
    {
        var result = await Mediator.Send(command);

        if (!result.IsSuccess)
            return FromResult(result);

        return OkResult(result.Data, LocalizationKeys.ActionResults.Updated);
    }

    /// <summary>
    /// Updates the subscription plans and their features displayed on the Home feed (admin only).
    /// Syncs existing, new, and removed plans and features.
    /// </summary>
    /// <param name="command">The list of subscription plans to update or create.</param>
    /// <returns>The updated list of subscription plans.</returns>
    /// <response code="200">Subscription plans updated successfully.</response>
    /// <response code="400">One or more fields are invalid.</response>
    /// <response code="401">The caller is not authenticated.</response>
    /// <response code="403">The caller is not authorized as Admin.</response>
    [HttpPut]
    [Route(ApiRoutes.Home.UpdatePlans)]
    [RoleAuthorize(UserType.Admin)]
    [ProducesResponseType(typeof(ApiResponse<List<HomePlanDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<List<HomePlanDto>>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdatePlans([FromBody] UpdateHomePlansCommand command)
    {
        var result = await Mediator.Send(command);

        if (!result.IsSuccess)
            return FromResult(result);

        return OkResult(result.Data, LocalizationKeys.ActionResults.Updated);
    }

    /// <summary>
    /// Updates the list of experts displayed on the Home feed (admin only).
    /// Syncs existing, new, and removed experts.
    /// </summary>
    /// <param name="command">The list of experts to update or create.</param>
    /// <returns>The updated list of experts.</returns>
    /// <response code="200">Experts updated successfully.</response>
    /// <response code="400">One or more fields are invalid.</response>
    /// <response code="401">The caller is not authenticated.</response>
    /// <response code="403">The caller is not authorized as Admin.</response>
    [HttpPut]
    [Route(ApiRoutes.Home.UpdateExperts)]
    [RoleAuthorize(UserType.Admin)]
    [ProducesResponseType(typeof(ApiResponse<List<HomeExpertDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<List<HomeExpertDto>>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateExperts([FromBody] UpdateHomeExpertsCommand command)
    {
        var result = await Mediator.Send(command);

        if (!result.IsSuccess)
            return FromResult(result);

        return OkResult(result.Data, LocalizationKeys.ActionResults.Updated);
    }
}
