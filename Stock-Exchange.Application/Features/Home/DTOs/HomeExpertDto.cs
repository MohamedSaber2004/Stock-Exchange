using Stock_Exchange.Domain.Enums;

namespace Stock_Exchange.Application.Features.Home.DTOs
{
    public class HomeExpertDto
    {
        public Guid Id { get; set; }
        public string FullNameEn { get; set; } = string.Empty;
        public string FullNameAr { get; set; } = string.Empty;
        public string TitleEn { get; set; } = string.Empty;
        public string TitleAr { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }

        public string FullName => !string.IsNullOrEmpty(FullNameEn) ? FullNameEn : FullNameAr;
        public string Title => !string.IsNullOrEmpty(TitleEn) ? TitleEn : TitleAr;

        public void ApplyLanguageFilter(Language language)
        {
            if (language == Language.en)
            {
                FullNameAr = string.Empty;
                TitleAr = string.Empty;
                return;
            }

            FullNameEn = string.Empty;
            TitleEn = string.Empty;
        }
    }
}
