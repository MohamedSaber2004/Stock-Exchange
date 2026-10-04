using Stock_Exchange.Domain.Enums;

namespace Stock_Exchange.Application.Features.ArticleCategories.DTOs
{
    public class ArticleCategoryDto
    {
        public Guid Id { get; set; }
        public string CategoryArName { get; set; } = string.Empty;
        public string CategoryEnName { get; set; } = string.Empty;
        public int ArticlesCount { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        public void ApplyLanguageFilter(Language language)
        {
            if (language == Language.en)
            {
                CategoryArName = string.Empty;
                return;
            }

            CategoryEnName = string.Empty;
        }
    }
}
