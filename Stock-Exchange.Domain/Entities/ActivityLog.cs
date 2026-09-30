using Stock_Exchange.Domain.Common;
using Stock_Exchange.Domain.Enums;

namespace Stock_Exchange.Domain.Entities
{
    public class ActivityLog : BaseEntity<Guid>
    {
        public string FormattedId { get; set; } = string.Empty;
        public Guid? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public string? UserProfilePictureUrl { get; set; }
        public string Action { get; set; } = string.Empty;
        public string ActionAr { get; set; } = string.Empty;
        public string ActionEn { get; set; } = string.Empty;
        public ActivityResourceType ResourceType { get; set; }
        public string IpAddress { get; set; } = string.Empty;
        public string Device { get; set; } = string.Empty;
        public string? Details { get; set; }
        public string? DetailsAr { get; set; }
        public string? DetailsEn { get; set; }
    }
}
