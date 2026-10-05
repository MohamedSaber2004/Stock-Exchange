namespace Stock_Exchange.Application.Features.Search.DTOs;

public class GlobalSearchItemDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Subtitle { get; set; }
    public string? Category { get; set; }
    public string? ImageUrl { get; set; }
    public string Type { get; set; } = string.Empty;
    public string TargetRoute { get; set; } = string.Empty;
    public DateTime? Date { get; set; }
    public string? Badge { get; set; }
}