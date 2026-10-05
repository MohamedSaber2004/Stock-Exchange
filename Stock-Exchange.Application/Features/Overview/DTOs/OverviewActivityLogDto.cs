namespace Stock_Exchange.Application.Features.Overview.DTOs;

public class OverviewActivityLogDto
{
    public Guid Id { get; set; }
    public Guid? UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public string? UserProfilePictureUrl { get; set; }
    public string Action { get; set; } = string.Empty;
    public string ActionAr { get; set; } = string.Empty;
    public string ActionEn { get; set; } = string.Empty;
    public string ResourceType { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
