using Stock_Exchange.Domain.Enums;

namespace Stock_Exchange.Application.Features.Home.DTOs
{
    public class HomeArticleDto
    {
        public Guid Id { get; set; }
        public string TitleEn { get; set; } = string.Empty;
        public string TitleAr { get; set; } = string.Empty;
        public string ExcerptEn { get; set; } = string.Empty;
        public string ExcerptAr { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public int ReadMinutes { get; set; }
        public DateTime PublishedAt { get; set; }

        public string Title => !string.IsNullOrEmpty(TitleEn) ? TitleEn : TitleAr;
        public string Excerpt => !string.IsNullOrEmpty(ExcerptEn) ? ExcerptEn : ExcerptAr;

        public void ApplyLanguageFilter(Language language)
        {
            if (language == Language.en)
            {
                TitleAr = string.Empty;
                ExcerptAr = string.Empty;
                return;
            }

            TitleEn = string.Empty;
            ExcerptEn = string.Empty;
        }
    }
}
