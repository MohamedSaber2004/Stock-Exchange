using Stock_Exchange.Domain.Enums;

namespace Stock_Exchange.Application.Features.Home.DTOs
{
    public class HomeDto
    {
        public string GreetingName { get; set; } = string.Empty;
        public HomeHeroDto Hero { get; set; } = new();
        public List<HomeNewsDto> LatestNews { get; set; } = new();
        public List<HomeServiceDto> Services { get; set; } = new();
        public List<HomeArticleDto> Articles { get; set; } = new();
        public List<HomeVideoDto> Videos { get; set; } = new();
        public List<HomePlanDto> Plans { get; set; } = new();
        public List<HomeExpertDto> Experts { get; set; } = new();

        public void ApplyLanguageFilter(Language language)
        {
            Hero.ApplyLanguageFilter(language);

            foreach (var news in LatestNews)
                news.ApplyLanguageFilter(language);

            foreach (var service in Services)
                service.ApplyLanguageFilter(language);

            foreach (var article in Articles)
                article.ApplyLanguageFilter(language);

            foreach (var video in Videos)
                video.ApplyLanguageFilter(language);

            foreach (var plan in Plans)
                plan.ApplyLanguageFilter(language);

            foreach (var expert in Experts)
                expert.ApplyLanguageFilter(language);
        }
    }
}
