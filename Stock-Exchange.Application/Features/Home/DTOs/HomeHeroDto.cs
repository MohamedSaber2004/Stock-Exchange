using Stock_Exchange.Domain.Enums;

namespace Stock_Exchange.Application.Features.Home.DTOs
{
    public class HomeHeroDto
    {
        public string TitleEn { get; set; } = string.Empty;
        public string TitleAr { get; set; } = string.Empty;
        public string SubtitleEn { get; set; } = string.Empty;
        public string SubtitleAr { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }

        public string Title => !string.IsNullOrEmpty(TitleEn) ? TitleEn : TitleAr;
        public string Subtitle => !string.IsNullOrEmpty(SubtitleEn) ? SubtitleEn : SubtitleAr;

        public void ApplyLanguageFilter(Language language)
        {
            if (language == Language.en)
            {
                TitleAr = string.Empty;
                SubtitleAr = string.Empty;
                return;
            }

            TitleEn = string.Empty;
            SubtitleEn = string.Empty;
        }
    }
}
