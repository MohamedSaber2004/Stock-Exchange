using Stock_Exchange.Domain.Enums;

namespace Stock_Exchange.Application.Features.HelpCenterCategories.DTOs
{
    public class HelpCenterCategoryDto
    {
        public Guid Id { get; set; }
        public string TitleEn { get; set; } = string.Empty;
        public string TitleAr { get; set; } = string.Empty;

        public void ApplyLanguageFilter(Language language)
        {
            if (language == Language.en)
            {
                TitleAr = string.Empty;
                return;
            }

            TitleEn = string.Empty;
        }
    }
}
