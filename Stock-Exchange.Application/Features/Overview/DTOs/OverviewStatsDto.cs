namespace Stock_Exchange.Application.Features.Overview.DTOs;

public class OverviewStatsDto
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int InactiveUsers { get; set; }
    public int NewUsersThisMonth { get; set; }
    public int TotalArticles { get; set; }
    public int TotalVideos { get; set; }
    public int TotalNews { get; set; }
    public int TotalServices { get; set; }
    public int TotalExperts { get; set; }
    public int TotalCountries { get; set; }
    public int TotalPlans { get; set; }
    public int TotalActivityLogs { get; set; }
}
