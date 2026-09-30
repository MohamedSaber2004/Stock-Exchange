using Stock_Exchange.Application.Common.Models;

namespace Stock_Exchange.Application.Features.ActivityLogs.DTOs
{
    public class ActivityLogsListResponse
    {
        public PagginatedResult<ActivityLogDto> Logs { get; set; } = null!;
        public ActivityLogsSummaryDto Summary { get; set; } = new();
    }
}
