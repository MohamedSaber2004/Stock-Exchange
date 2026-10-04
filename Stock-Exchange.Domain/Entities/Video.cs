using Stock_Exchange.Domain.Common;

namespace Stock_Exchange.Domain.Entities
{
    public class Video : BaseEntity<Guid>
    {
        public string TitleEn { get; set; } = string.Empty;
        public string TitleAr { get; set; } = string.Empty;
        public string? ThumbnailUrl { get; set; }
        public string? VideoUrl { get; set; }
        public int DurationSeconds { get; set; }
        public string InstructorName { get; set; } = string.Empty;
        public string CategoryEn { get; set; } = string.Empty;
        public string CategoryAr { get; set; } = string.Empty;
        public bool IsPreviewable { get; set; } = true;
        public bool IsFeaturedOnHome { get; set; } = true;
        public int DisplayOrder { get; set; }

        public Guid? CategoryId { get; set; }
        public virtual VideoCategory? Category { get; set; }
    }
}
