using Stock_Exchange.Domain.Enums;

namespace Stock_Exchange.Application.Features.Home.DTOs
{
    public class HomeNewsDto
    {
        public Guid Id { get; set; }
        public string TitleEn { get; set; } = string.Empty;
        public string TitleAr { get; set; } = string.Empty;
        public string SummaryEn { get; set; } = string.Empty;
        public string SummaryAr { get; set; } = string.Empty;
        public string CategoryEn { get; set; } = string.Empty;
        public string CategoryAr { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public DateTime PublishedAt { get; set; }

        public string Title => !string.IsNullOrEmpty(TitleEn) ? TitleEn : TitleAr;
        public string Summary => !string.IsNullOrEmpty(SummaryEn) ? SummaryEn : SummaryAr;
        public string Category => !string.IsNullOrEmpty(CategoryEn) ? CategoryEn : CategoryAr;

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
