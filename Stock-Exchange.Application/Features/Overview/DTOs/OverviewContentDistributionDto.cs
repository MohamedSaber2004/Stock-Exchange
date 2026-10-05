namespace Stock_Exchange.Application.Features.Overview.DTOs;

public class OverviewContentDistributionDto
{
    public int ArticlesCount { get; set; }
    public int VideosCount { get; set; }
    public int NewsCount { get; set; }
    public int ServicesCount { get; set; }
    public int TotalContentItems => ArticlesCount + VideosCount + NewsCount + ServicesCount;

    public double ArticlesPercentage => TotalContentItems == 0 ? 0 : Math.Round((double)ArticlesCount / TotalContentItems * 100, 1);
    public double VideosPercentage => TotalContentItems == 0 ? 0 : Math.Round((double)VideosCount / TotalContentItems * 100, 1);
    public double NewsPercentage => TotalContentItems == 0 ? 0 : Math.Round((double)NewsCount / TotalContentItems * 100, 1);
    public double ServicesPercentage => TotalContentItems == 0 ? 0 : Math.Round((double)ServicesCount / TotalContentItems * 100, 1);
}
