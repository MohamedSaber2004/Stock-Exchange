using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.Overview.DTOs;
using Stock_Exchange.Application.Features.Overview.Queries.GetAdminOverview;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Enums;
using Stock_Exchange.Filters;
using Stock_Exchange.Routes.V1;

namespace Stock_Exchange.Controllers.V1;

[ApiVersion(1.0)]
[Route(ApiRoutes.Overview.Base)]
[RoleAuthorize(UserType.Admin)]
public class OverviewController : BaseController
{
    /// <summary>
    /// Retrieves comprehensive overview statistics, trends, and recent activities for the admin dashboard.
    /// </summary>
    [HttpGet]
    [Route(ApiRoutes.Overview.Get)]
    [ProducesResponseType(typeof(ApiResponse<AdminOverviewDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOverview()
    {
        var result = await Mediator.Send(new GetAdminOverviewQuery());
        if (!result.IsSuccess)
            return FromResult(result);

        return OkResult(result.Data, LocalizationKeys.ActionResults.Ok);
    }
}