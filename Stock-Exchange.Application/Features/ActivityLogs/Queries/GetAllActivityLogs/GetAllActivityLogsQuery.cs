using MediatR;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.ActivityLogs.DTOs;
using Stock_Exchange.Domain.Enums;

namespace Stock_Exchange.Application.Features.ActivityLogs.Queries.GetAllActivityLogs
{
    public record GetAllActivityLogsQuery : IRequest<Result<ActivityLogsListResponse>>
    {
        public string? Search { get; init; }
        public ActivityResourceType? ResourceType { get; init; }
        public int PageNumber { get; init; } = 1;
        public int PageSize { get; init; } = 10;
    }
}
