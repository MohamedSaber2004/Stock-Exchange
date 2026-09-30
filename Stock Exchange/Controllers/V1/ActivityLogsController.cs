using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.ActivityLogs.DTOs;
using Stock_Exchange.Application.Features.ActivityLogs.Queries.GetAllActivityLogs;
using Stock_Exchange.Application.Features.ActivityLogs.Queries.GetActivityLogById;
using Stock_Exchange.Application.Features.ActivityLogs.Queries.GetActivityLogsSummary;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Enums;
using Stock_Exchange.Filters;
using Stock_Exchange.Routes.V1;

namespace Stock_Exchange.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route(ApiRoutes.ActivityLogs.Base)]
    [RoleAuthorize(UserType.Admin)]
    public class ActivityLogsController : BaseController
    {
        /// <summary>
        /// Retrieves paginated activity logs along with summary statistics for the admin dashboard.
        /// </summary>
        /// <param name="query">Search term, resource type filter, and pagination parameters.</param>
        /// <returns>Paginated activity logs and top summary metrics.</returns>
        [HttpGet]
        [Route(ApiRoutes.ActivityLogs.GetAll)]
        [ProducesResponseType(typeof(ApiResponse<ActivityLogsListResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] GetAllActivityLogsQuery query)
        {
            var result = await Mediator.Send(query);
            if (!result.IsSuccess)
                return FromResult(result);

            return OkResult(result.Data, LocalizationKeys.ActionResults.Ok);
        }

        /// <summary>
        /// Retrieves summary statistics for the activity logs cards (Total, User Registrations, Articles, Videos, Content Ops).
        /// </summary>
        /// <returns>Activity logs summary counts.</returns>
        [HttpGet]
        [Route(ApiRoutes.ActivityLogs.Summary)]
        [ProducesResponseType(typeof(ApiResponse<ActivityLogsSummaryDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSummary()
        {
            var result = await Mediator.Send(new GetActivityLogsSummaryQuery());
            if (!result.IsSuccess)
                return FromResult(result);

            return OkResult(result.Data, LocalizationKeys.ActionResults.Ok);
        }

        /// <summary>
        /// Retrieves a specific activity log event by its ID or formatted code (e.g. act-001).
        /// Used for the Event Data Inspector modal ("فاحص بيانات الحدث").
        /// </summary>
        /// <param name="id">The log Guid or formatted code (e.g. act-001).</param>
        /// <returns>Activity log details.</returns>
        [HttpGet]
        [Route(ApiRoutes.ActivityLogs.GetById)]
        [ProducesResponseType(typeof(ApiResponse<ActivityLogDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await Mediator.Send(new GetActivityLogByIdQuery(id));
            if (!result.IsSuccess)
                return FromResult(result);

            return OkResult(result.Data, LocalizationKeys.ActionResults.Ok);
        }
    }
}
