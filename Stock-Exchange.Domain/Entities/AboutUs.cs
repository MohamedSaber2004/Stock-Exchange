using Stock_Exchange.Domain.Common;

namespace Stock_Exchange.Domain.Entities
{
    public class AboutUs : BaseEntity<Guid>
    {
        public string StoryEn { get; set; } = string.Empty;
        public string StoryAr { get; set; } = string.Empty;
        public string MissionEn { get; set; } = string.Empty;
        public string MissionAr { get; set; } = string.Empty;
        public string VisionEn { get; set; } = string.Empty;
        public string VisionAr { get; set; } = string.Empty;
        public string? SupportEmail { get; set; }

        public virtual ICollection<AboutUsFeature> Features { get; set; } = new List<AboutUsFeature>();
    }
}
