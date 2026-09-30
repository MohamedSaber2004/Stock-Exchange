using MediatR;
using Microsoft.EntityFrameworkCore;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.ActivityLogs.DTOs;
using Stock_Exchange.Domain.Enums;

namespace Stock_Exchange.Application.Features.ActivityLogs.Queries.GetActivityLogsSummary
{
    public record GetActivityLogsSummaryQuery : IRequest<Result<ActivityLogsSummaryDto>>;

    public class GetActivityLogsSummaryQueryHandler : IRequestHandler<GetActivityLogsSummaryQuery, Result<ActivityLogsSummaryDto>>
    {
        private readonly IStockExchangeDbContext _dbContext;
        private readonly ICurrentLanguageService _currentLanguageService;

        public GetActivityLogsSummaryQueryHandler(
            IStockExchangeDbContext dbContext,
            ICurrentLanguageService currentLanguageService)
        {
            _dbContext = dbContext;
            _currentLanguageService = currentLanguageService;
        }

        public async Task<Result<ActivityLogsSummaryDto>> Handle(GetActivityLogsSummaryQuery request, CancellationToken cancellationToken)
        {
            var baseQuery = _dbContext.ActivityLogs
                .AsNoTracking()
                .Where(a => !a.IsDeleted);

            var totalLogs = await baseQuery.CountAsync(cancellationToken);
            var userRegistrations = await baseQuery
                .Where(a => a.ResourceType == ActivityResourceType.UserRegistrations)
                .CountAsync(cancellationToken);
            var articles = await baseQuery
                .Where(a => a.ResourceType == ActivityResourceType.Articles)
                .CountAsync(cancellationToken);
            var videos = await baseQuery
                .Where(a => a.ResourceType == ActivityResourceType.Videos)
                .CountAsync(cancellationToken);
            var contentOps = await baseQuery
                .Where(a => a.ResourceType == ActivityResourceType.Articles ||
                            a.ResourceType == ActivityResourceType.Videos ||
                            a.ResourceType == ActivityResourceType.News)
                .CountAsync(cancellationToken);
            var usersOps = await baseQuery
                .Where(a => a.ResourceType == ActivityResourceType.Users)
                .CountAsync(cancellationToken);

            var summary = ActivityLogsSummaryDto.Create(
                totalLogs,
                userRegistrations,
                articles,
                videos,
                contentOps,
                usersOps,
                _currentLanguageService.Language);

            return Result<ActivityLogsSummaryDto>.Success(summary);
        }
    }
}
