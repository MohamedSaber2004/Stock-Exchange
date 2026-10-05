using Stock_Exchange.Domain.Common;

namespace Stock_Exchange.Domain.Entities
{
    public class News : BaseEntity<Guid>
    {
        public string TitleEn { get; set; } = string.Empty;
        public string TitleAr { get; set; } = string.Empty;
        public string SummaryEn { get; set; } = string.Empty;
        public string SummaryAr { get; set; } = string.Empty;
        public string ContentEn { get; set; } = string.Empty;
        public string ContentAr { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public string CategoryEn { get; set; } = string.Empty;
        public string CategoryAr { get; set; } = string.Empty;
        public DateTime PublishedAt { get; set; } = DateTime.UtcNow;
        public int DisplayOrder { get; set; }
        public bool IsFeaturedOnHome { get; set; } = true;
    }
}
