using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.SubscriptionPlans.DTOs;
using Stock_Exchange.Application.Features.SubscriptionPlans.Queries.GetAllSubscriptionPlans;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Filters;
using Stock_Exchange.Routes.V1;

namespace Stock_Exchange.Controllers.V1;

[ApiVersion("1.0")]
[Route(ApiRoutes.SubscriptionPlans.Base)]
[Route("api/v{version:apiVersion}/subscriptions")]
public class SubscriptionPlansController : BaseController
{
    /// <summary>
    /// Retrieves all active subscription plans along with their features.
    /// General authorization: Accessible to any authenticated user.
    /// </summary>
    /// <param name="query">Optional filter parameters (isActive, applyLanguageFilter).</param>
    /// <returns>A list of subscription plans.</returns>
    /// <response code="200">Subscription plans retrieved successfully.</response>
    [HttpGet]
    [Route(ApiRoutes.SubscriptionPlans.GetAll)]
    [RoleAuthorize]
    [ProducesResponseType(typeof(ApiResponse<List<SubscriptionPlanDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] GetAllSubscriptionPlansQuery query)
    {
        var result = await Mediator.Send(query);

        if (!result.IsSuccess)
            return FromResult(result);

        return OkResult(result.Data, LocalizationKeys.ActionResults.Ok);
    }
}
