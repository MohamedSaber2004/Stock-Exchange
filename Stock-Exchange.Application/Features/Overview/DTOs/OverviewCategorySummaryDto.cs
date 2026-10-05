namespace Stock_Exchange.Application.Features.Overview.DTOs;

public class OverviewCategorySummaryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public int Count { get; set; }
}
