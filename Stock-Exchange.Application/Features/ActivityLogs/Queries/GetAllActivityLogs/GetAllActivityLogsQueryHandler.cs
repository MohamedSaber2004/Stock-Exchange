using MediatR;
using Microsoft.EntityFrameworkCore;
using Stock_Exchange.Application.Common.Extensions;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.ActivityLogs.DTOs;
using Stock_Exchange.Domain.Enums;

namespace Stock_Exchange.Application.Features.ActivityLogs.Queries.GetAllActivityLogs
{
    public class GetAllActivityLogsQueryHandler : IRequestHandler<GetAllActivityLogsQuery, Result<ActivityLogsListResponse>>
    {
        private readonly IStockExchangeDbContext _dbContext;
        private readonly ICurrentLanguageService _currentLanguageService;

        public GetAllActivityLogsQueryHandler(
            IStockExchangeDbContext dbContext,
            ICurrentLanguageService currentLanguageService)
        {
            _dbContext = dbContext;
            _currentLanguageService = currentLanguageService;
        }

        public async Task<Result<ActivityLogsListResponse>> Handle(GetAllActivityLogsQuery request, CancellationToken cancellationToken)
        {
            var baseQuery = _dbContext.ActivityLogs
                .AsNoTracking()
                .Where(a => !a.IsDeleted);

            // Compute summary statistics
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

            var language = _currentLanguageService.Language;
            var summary = ActivityLogsSummaryDto.Create(
                totalLogs,
                userRegistrations,
                articles,
                videos,
                contentOps,
                usersOps,
                language);

            var query = baseQuery;

            if (request.ResourceType.HasValue)
            {
                query = query.Where(a => a.ResourceType == request.ResourceType.Value);
            }

            var search = request.Search?.Trim();
            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.ToLower();
                query = query.Where(a =>
                    a.UserName.ToLower().Contains(term) ||
                    a.UserEmail.ToLower().Contains(term) ||
                    a.Action.ToLower().Contains(term) ||
                    a.ActionAr.ToLower().Contains(term) ||
                    a.ActionEn.ToLower().Contains(term) ||
                    a.IpAddress.ToLower().Contains(term) ||
                    a.FormattedId.ToLower().Contains(term));
            }

            var safePageSize = request.PageSize <= 0 ? 10 : request.PageSize;
            var safePageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;

            var pagedLogs = await query
                .OrderByDescending(a => a.CreatedAt)
                .Select(a => new ActivityLogDto
                {
                    Id = a.Id,
                    FormattedId = a.FormattedId,
                    UserId = a.UserId,
                    UserName = a.UserName,
                    UserEmail = a.UserEmail,
                    UserProfilePictureUrl = a.UserProfilePictureUrl,
                    Action = a.Action,
                    ActionAr = a.ActionAr,
                    ActionEn = a.ActionEn,
                    ResourceType = a.ResourceType,
                    IpAddress = a.IpAddress,
                    Device = a.Device,
                    CreatedAt = a.CreatedAt,
                    Details = a.Details,
                    DetailsAr = a.DetailsAr,
                    DetailsEn = a.DetailsEn
                })
                .AsPagginatedListAsync(safePageNumber, safePageSize, cancellationToken);

            foreach (var item in pagedLogs.Items)
            {
                item.ApplyLanguageFilter(language);
            }

            var response = new ActivityLogsListResponse
            {
                Logs = pagedLogs,
                Summary = summary
            };

            return Result<ActivityLogsListResponse>.Success(response);
        }
    }
}
