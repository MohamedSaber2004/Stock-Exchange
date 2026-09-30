using Stock_Exchange.Domain.Common;

namespace Stock_Exchange.Domain.Entities
{
    public class Article : BaseEntity<Guid>
    {
        public string TitleEn { get; set; } = string.Empty;
        public string TitleAr { get; set; } = string.Empty;
        public string ExcerptEn { get; set; } = string.Empty;
        public string ExcerptAr { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public int ReadMinutes { get; set; } = 5;
        public DateTime PublishedAt { get; set; } = DateTime.UtcNow;
        public bool IsFeaturedOnHome { get; set; } = true;
        public int DisplayOrder { get; set; }
    }
}
