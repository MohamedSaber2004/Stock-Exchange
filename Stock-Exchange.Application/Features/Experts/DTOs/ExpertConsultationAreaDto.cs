using Stock_Exchange.Domain.Enums;

namespace Stock_Exchange.Application.Features.Experts.DTOs
{
    public class ExpertConsultationAreaDto
    {
        public Guid Id { get; set; }
        public string TextEn { get; set; } = string.Empty;
        public string TextAr { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }

        public string Text => !string.IsNullOrEmpty(TextEn) ? TextEn : TextAr;

        public void ApplyLanguageFilter(Language language)
        {
            if (language == Language.en)
            {
                TextAr = string.Empty;
                return;
            }

            TextEn = string.Empty;
        }
    }

    public class ExpertConsultationAreaInputDto
    {
        public Guid? Id { get; set; }
        public string TextEn { get; set; } = string.Empty;
        public string TextAr { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
    }
}
