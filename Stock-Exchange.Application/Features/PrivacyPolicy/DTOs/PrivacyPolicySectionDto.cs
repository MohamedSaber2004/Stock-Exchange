using Stock_Exchange.Domain.Enums;

namespace Stock_Exchange.Application.Features.PrivacyPolicy.DTOs
{
    public class PrivacyPolicySectionDto
    {
        public Guid Id { get; set; }
        public string TitleEn { get; set; } = string.Empty;
        public string TitleAr { get; set; } = string.Empty;
        public string ContentEn { get; set; } = string.Empty;
        public string ContentAr { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }

        public void ApplyLanguageFilter(Language language)
        {
            if (language == Language.en)
            {
                TitleAr = string.Empty;
                ContentAr = string.Empty;
                return;
            }

            TitleEn = string.Empty;
            ContentEn = string.Empty;
        }
    }
}
