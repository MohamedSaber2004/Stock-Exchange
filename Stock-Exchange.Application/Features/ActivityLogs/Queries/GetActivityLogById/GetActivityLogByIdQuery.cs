using MediatR;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.ActivityLogs.DTOs;

namespace Stock_Exchange.Application.Features.ActivityLogs.Queries.GetActivityLogById
{
    public record GetActivityLogByIdQuery(string Id) : IRequest<Result<ActivityLogDto>>;
}
