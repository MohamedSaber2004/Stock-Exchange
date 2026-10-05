using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.Search.DTOs;
using Stock_Exchange.Application.Features.Search.Queries.GlobalSearch;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Enums;
using Stock_Exchange.Filters;
using Stock_Exchange.Routes.V1;

namespace Stock_Exchange.Controllers.V1;

[ApiVersion(1.0)]
[Route(ApiRoutes.Search.Base)]
[RoleAuthorize(UserType.Admin)]
public class SearchController : BaseController
{
    /// <summary>
    /// Performs a global search across articles, videos, news, users, services, experts, and countries for the admin portal.
    /// </summary>
    [HttpGet]
    [Route(ApiRoutes.Search.Global)]
    [ProducesResponseType(typeof(ApiResponse<GlobalSearchResultDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GlobalSearch([FromQuery] string? query, [FromQuery] int limit = 5)
    {
        var result = await Mediator.Send(new GlobalSearchQuery(query ?? string.Empty, limit));
        if (!result.IsSuccess)
            return FromResult(result);

        return OkResult(result.Data, LocalizationKeys.ActionResults.Ok);
    }
}