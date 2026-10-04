using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.VideoCategories.DTOs;
using Stock_Exchange.Application.Features.VideoCategories.Queries.GetAllVideoCategories;
using Stock_Exchange.Application.Localization;
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
}
