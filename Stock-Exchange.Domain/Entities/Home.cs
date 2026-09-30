using Stock_Exchange.Domain.Common;

namespace Stock_Exchange.Domain.Entities
{
    public class Home : BaseEntity<Guid>
    {
        public string HeroTitleEn { get; set; } = string.Empty;
        public string HeroTitleAr { get; set; } = string.Empty;
        public string HeroSubtitleEn { get; set; } = string.Empty;
        public string HeroSubtitleAr { get; set; } = string.Empty;
        public string? HeroImageUrl { get; set; }
    }
}
