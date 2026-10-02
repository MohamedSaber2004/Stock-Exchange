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
        private readonly ICurrentUserService _currentUserService;

        public GetActivityLogsSummaryQueryHandler(
            IStockExchangeDbContext dbContext,
            ICurrentLanguageService currentLanguageService,
            ICurrentUserService currentUserService)
        {
            _dbContext = dbContext;
            _currentLanguageService = currentLanguageService;
            _currentUserService = currentUserService;
        }

        public async Task<Result<ActivityLogsSummaryDto>> Handle(GetActivityLogsSummaryQuery request, CancellationToken cancellationToken)
        {
            var baseQuery = _dbContext.ActivityLogs
                .AsNoTracking()
                .Where(a => !a.IsDeleted);

            // Exclude current authenticated user's activity logs
            if (_currentUserService.IsAuthenticated)
            {
                var currentUserId = _currentUserService.UserId;
                var currentEmail = _currentUserService.Email?.Trim().ToLower();

                if (currentUserId != Guid.Empty)
                {
                    baseQuery = baseQuery.Where(a => a.UserId != currentUserId);
                }

                if (!string.IsNullOrEmpty(currentEmail))
                {
                    baseQuery = baseQuery.Where(a => a.UserEmail.ToLower() != currentEmail);
                }
            }

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

            // Total users registered in the system (matches Users Management count)
            var totalUsers = await _dbContext.Users
                .AsNoTracking()
                .Where(u => !u.IsDeleted)
                .CountAsync(cancellationToken);

            var summary = ActivityLogsSummaryDto.Create(
                totalLogs,
                userRegistrations,
                articles,
                videos,
                contentOps,
                totalUsers,
                _currentLanguageService.Language);

            return Result<ActivityLogsSummaryDto>.Success(summary);
        }
    }
}
