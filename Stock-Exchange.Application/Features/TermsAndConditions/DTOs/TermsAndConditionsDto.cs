using Stock_Exchange.Domain.Enums;

namespace Stock_Exchange.Application.Features.TermsAndConditions.DTOs
{
    public class TermsAndConditionsDto
    {
        public Guid Id { get; set; }
        public string TitleEn { get; set; } = string.Empty;
        public string TitleAr { get; set; } = string.Empty;
        public string? DescriptionEn { get; set; }
        public string? DescriptionAr { get; set; }
        public List<TermsAndConditionsSectionDto> Sections { get; set; } = new();

        public void ApplyLanguageFilter(Language language)
        {
            if (language == Language.en)
            {
                TitleAr = string.Empty;
                DescriptionAr = string.Empty;
                foreach (var section in Sections)
                {
                    section.ApplyLanguageFilter(language);
                }
                return;
            }

            TitleEn = string.Empty;
            DescriptionEn = string.Empty;
            foreach (var section in Sections)
            {
                section.ApplyLanguageFilter(language);
            }
        }
    }
}
