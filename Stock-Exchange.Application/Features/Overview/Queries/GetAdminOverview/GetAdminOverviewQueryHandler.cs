using MediatR;
using Microsoft.EntityFrameworkCore;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.Overview.DTOs;
using Stock_Exchange.Domain.Enums;

namespace Stock_Exchange.Application.Features.Overview.Queries.GetAdminOverview;

public class GetAdminOverviewQueryHandler : IRequestHandler<GetAdminOverviewQuery, Result<AdminOverviewDto>>
{
    private readonly IStockExchangeDbContext _dbContext;
    private readonly ICurrentLanguageService _currentLanguageService;

    public GetAdminOverviewQueryHandler(
        IStockExchangeDbContext dbContext,
        ICurrentLanguageService currentLanguageService)
    {
        _dbContext = dbContext;
        _currentLanguageService = currentLanguageService;
    }

    public async Task<Result<AdminOverviewDto>> Handle(GetAdminOverviewQuery request, CancellationToken cancellationToken)
    {
        var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);
        var isArabic = _currentLanguageService.Language == Language.ar;

        // Metric Counts
        var totalUsers = await _dbContext.Users.AsNoTracking().CountAsync(u => !u.IsDeleted, cancellationToken);
        var activeUsers = await _dbContext.Users.AsNoTracking().CountAsync(u => !u.IsDeleted && u.IsActive, cancellationToken);
        var newUsersThisMonth = await _dbContext.Users.AsNoTracking().CountAsync(u => !u.IsDeleted && u.CreatedAt >= thirtyDaysAgo, cancellationToken);

        var totalArticles = await _dbContext.Articles.AsNoTracking().CountAsync(a => !a.IsDeleted, cancellationToken);
        var totalVideos = await _dbContext.Videos.AsNoTracking().CountAsync(v => !v.IsDeleted, cancellationToken);
        var totalNews = await _dbContext.News.AsNoTracking().CountAsync(n => !n.IsDeleted, cancellationToken);
        var totalServices = await _dbContext.Services.AsNoTracking().CountAsync(s => !s.IsDeleted, cancellationToken);
        var totalExperts = await _dbContext.Experts.AsNoTracking().CountAsync(e => !e.IsDeleted, cancellationToken);
        var totalCountries = await _dbContext.Countries.AsNoTracking().CountAsync(c => !c.IsDeleted, cancellationToken);
        var totalPlans = await _dbContext.SubscriptionPlans.AsNoTracking().CountAsync(p => !p.IsDeleted, cancellationToken);
        var totalActivityLogs = await _dbContext.ActivityLogs.AsNoTracking().CountAsync(a => !a.IsDeleted, cancellationToken);

        var stats = new OverviewStatsDto
        {
            TotalUsers = totalUsers,
            ActiveUsers = activeUsers,
            InactiveUsers = Math.Max(0, totalUsers - activeUsers),
            NewUsersThisMonth = newUsersThisMonth,
            TotalArticles = totalArticles,
            TotalVideos = totalVideos,
            TotalNews = totalNews,
            TotalServices = totalServices,
            TotalExperts = totalExperts,
            TotalCountries = totalCountries,
            TotalPlans = totalPlans,
            TotalActivityLogs = totalActivityLogs
        };

        // Content Distribution
        var contentDistribution = new OverviewContentDistributionDto
        {
            ArticlesCount = totalArticles,
            VideosCount = totalVideos,
            NewsCount = totalNews,
            ServicesCount = totalServices
        };

        // Trend over last 7 days
        var startDate = DateTime.UtcNow.Date.AddDays(-6);

        var userRegistrationDates = await _dbContext.Users.AsNoTracking()
            .Where(u => !u.IsDeleted && u.CreatedAt >= startDate)
            .Select(u => u.CreatedAt)
            .ToListAsync(cancellationToken);

        var activityLogDates = await _dbContext.ActivityLogs.AsNoTracking()
            .Where(a => !a.IsDeleted && a.CreatedAt >= startDate)
            .Select(a => a.CreatedAt)
            .ToListAsync(cancellationToken);

        var userTrend = new List<OverviewTrendItemDto>();
        var activityTrend = new List<OverviewTrendItemDto>();

        for (int i = 0; i < 7; i++)
        {
            var date = startDate.AddDays(i);
            var dayName = date.ToString("ddd");
            var dateStr = date.ToString("yyyy-MM-dd");

            userTrend.Add(new OverviewTrendItemDto
            {
                Date = dateStr,
                DayName = dayName,
                Count = userRegistrationDates.Count(d => d.Date == date)
            });

            activityTrend.Add(new OverviewTrendItemDto
            {
                Date = dateStr,
                DayName = dayName,
                Count = activityLogDates.Count(d => d.Date == date)
            });
        }

        // Recent Activity Logs (top 8)
        var recentLogsEntities = await _dbContext.ActivityLogs.AsNoTracking()
            .Where(a => !a.IsDeleted)
            .OrderByDescending(a => a.CreatedAt)
            .Take(8)
            .Select(a => new
            {
                a.Id,
                a.UserId,
                a.UserName,
                a.UserEmail,
                a.UserProfilePictureUrl,
                a.Action,
                a.ActionAr,
                a.ActionEn,
                a.ResourceType,
                a.CreatedAt
            })
            .ToListAsync(cancellationToken);

        var recentActivities = recentLogsEntities.Select(a => new OverviewActivityLogDto
        {
            Id = a.Id,
            UserId = a.UserId,
            UserName = !string.IsNullOrEmpty(a.UserName) ? a.UserName : "Admin",
            UserEmail = a.UserEmail ?? string.Empty,
            UserProfilePictureUrl = a.UserProfilePictureUrl,
            Action = (isArabic ? (a.ActionAr ?? a.Action) : (a.ActionEn ?? a.Action)) ?? string.Empty,
            ActionAr = a.ActionAr ?? a.Action ?? string.Empty,
            ActionEn = a.ActionEn ?? a.Action ?? string.Empty,
            ResourceType = a.ResourceType.ToString(),
            CreatedAt = a.CreatedAt
        }).ToList();

        // Top Article Categories
        var articleCatEntities = await _dbContext.ArticleCategories.AsNoTracking()
            .Where(c => !c.IsDeleted)
            .Select(c => new
            {
                c.Id,
                c.CategoryArName,
                c.CategoryEnName,
                Count = c.Articles.Count(a => !a.IsDeleted)
            })
            .OrderByDescending(c => c.Count)
            .Take(5)
            .ToListAsync(cancellationToken);

        var topArticleCategories = articleCatEntities.Select(c => new OverviewCategorySummaryDto
        {
            Id = c.Id,
            Name = (isArabic ? c.CategoryArName : c.CategoryEnName) ?? string.Empty,
            NameAr = c.CategoryArName ?? string.Empty,
            NameEn = c.CategoryEnName ?? string.Empty,
            Count = c.Count
        }).ToList();

        // Top Video Categories
        var videoCatEntities = await _dbContext.VideoCategories.AsNoTracking()
            .Where(c => !c.IsDeleted)
            .Select(c => new
            {
                c.Id,
                c.CategoryArName,
                c.CategoryEnName,
                Count = c.Videos.Count(v => !v.IsDeleted)
            })
            .OrderByDescending(c => c.Count)
            .Take(5)
            .ToListAsync(cancellationToken);

        var topVideoCategories = videoCatEntities.Select(c => new OverviewCategorySummaryDto
        {
            Id = c.Id,
            Name = (isArabic ? c.CategoryArName : c.CategoryEnName) ?? string.Empty,
            NameAr = c.CategoryArName ?? string.Empty,
            NameEn = c.CategoryEnName ?? string.Empty,
            Count = c.Count
        }).ToList();

        var overview = new AdminOverviewDto
        {
            Stats = stats,
            ContentDistribution = contentDistribution,
            UserRegistrationTrend = userTrend,
            ActivityTrend = activityTrend,
            RecentActivities = recentActivities,
            TopArticleCategories = topArticleCategories,
            TopVideoCategories = topVideoCategories
        };

        return Result<AdminOverviewDto>.Success(overview);
    }
}
