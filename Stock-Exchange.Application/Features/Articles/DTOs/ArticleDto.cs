using Stock_Exchange.Domain.Enums;

namespace Stock_Exchange.Application.Features.Articles.DTOs
{
    public class ArticleDto
    {
        public Guid Id { get; set; }
        public string TitleEn { get; set; } = string.Empty;
        public string TitleAr { get; set; } = string.Empty;
        public string ExcerptEn { get; set; } = string.Empty;
        public string ExcerptAr { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public DateTime PublishedAt { get; set; }
        public bool IsFeaturedOnHome { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        public Guid? CategoryId { get; set; }
        public Guid? ArticleCategoryId { get; set; }
        public string? CategoryEnName { get; set; }
        public string? CategoryArName { get; set; }

        public string Title => !string.IsNullOrEmpty(TitleEn) ? TitleEn : TitleAr;
        public string Excerpt => !string.IsNullOrEmpty(ExcerptEn) ? ExcerptEn : ExcerptAr;

        public void ApplyLanguageFilter(Language language)
        {
            if (language == Language.en)
            {
                TitleAr = string.Empty;
                ExcerptAr = string.Empty;
                CategoryArName = null;
                return;
            }

            TitleEn = string.Empty;
            ExcerptEn = string.Empty;
            CategoryEnName = null;
        }
    }
}
