namespace Stock_Exchange.Application.Features.Overview.DTOs;

public class OverviewTrendItemDto
{
    public string Date { get; set; } = string.Empty;
    public string DayName { get; set; } = string.Empty;
    public int Count { get; set; }
}
