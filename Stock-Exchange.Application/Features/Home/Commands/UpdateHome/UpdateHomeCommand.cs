using MediatR;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.Home.DTOs;

namespace Stock_Exchange.Application.Features.Home.Commands.UpdateHome
{
    public class UpdateHomeCommand : IRequest<Result<HomeHeroDto>>
    {
        public string? HeroTitleEn { get; set; }
        public string? HeroTitleAr { get; set; }
        public string? HeroSubtitleEn { get; set; }
        public string? HeroSubtitleAr { get; set; }
        public string? HeroImageUrl { get; set; }

        public UpdateHomeCommand()
        {
        }

        public UpdateHomeCommand(
            string? heroTitleEn = null,
            string? heroTitleAr = null,
            string? heroSubtitleEn = null,
            string? heroSubtitleAr = null,
            string? heroImageUrl = null)
        {
            HeroTitleEn = heroTitleEn;
            HeroTitleAr = heroTitleAr;
            HeroSubtitleEn = heroSubtitleEn;
            HeroSubtitleAr = heroSubtitleAr;
            HeroImageUrl = heroImageUrl;
        }
    }
}
