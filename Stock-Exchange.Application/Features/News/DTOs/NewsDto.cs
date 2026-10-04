using Stock_Exchange.Domain.Enums;

namespace Stock_Exchange.Application.Features.News.DTOs
{
    public class NewsDto
    {
        public Guid Id { get; set; }
        public string TitleEn { get; set; } = string.Empty;
        public string TitleAr { get; set; } = string.Empty;
        public string SummaryEn { get; set; } = string.Empty;
        public string SummaryAr { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public string CategoryEn { get; set; } = string.Empty;
        public string CategoryAr { get; set; } = string.Empty;
        public DateTime PublishedAt { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsFeaturedOnHome { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        public string Title => !string.IsNullOrEmpty(TitleEn) ? TitleEn : TitleAr;
        public string Summary => !string.IsNullOrEmpty(SummaryEn) ? SummaryEn : SummaryAr;

        public void ApplyLanguageFilter(Language language)
        {
            if (language == Language.en)
            {
                TitleAr = string.Empty;
                SummaryAr = string.Empty;
                CategoryAr = string.Empty;
                return;
            }

            TitleEn = string.Empty;
            SummaryEn = string.Empty;
            CategoryEn = string.Empty;
        }
    }
}
