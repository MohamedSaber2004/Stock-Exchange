namespace Stock_Exchange.Application.Features.Search.DTOs;

public class GlobalSearchResultDto
{
    public string Query { get; set; } = string.Empty;
    public int TotalCount { get; set; }
    public List<GlobalSearchItemDto> Items { get; set; } = new();
    public List<GlobalSearchItemDto> Articles { get; set; } = new();
    public List<GlobalSearchItemDto> Videos { get; set; } = new();
    public List<GlobalSearchItemDto> News { get; set; } = new();
    public List<GlobalSearchItemDto> Users { get; set; } = new();
    public List<GlobalSearchItemDto> Services { get; set; } = new();
    public List<GlobalSearchItemDto> Experts { get; set; } = new();
    public List<GlobalSearchItemDto> Countries { get; set; } = new();
    public List<GlobalSearchItemDto> HelpCenter { get; set; } = new();
}