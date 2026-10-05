namespace Stock_Exchange.Application.Features.Overview.DTOs;

public class AdminOverviewDto
{
    public OverviewStatsDto Stats { get; set; } = new();
    public OverviewContentDistributionDto ContentDistribution { get; set; } = new();
    public List<OverviewTrendItemDto> UserRegistrationTrend { get; set; } = new();
    public List<OverviewTrendItemDto> ActivityTrend { get; set; } = new();
    public List<OverviewActivityLogDto> RecentActivities { get; set; } = new();
    public List<OverviewCategorySummaryDto> TopArticleCategories { get; set; } = new();
    public List<OverviewCategorySummaryDto> TopVideoCategories { get; set; } = new();
}
