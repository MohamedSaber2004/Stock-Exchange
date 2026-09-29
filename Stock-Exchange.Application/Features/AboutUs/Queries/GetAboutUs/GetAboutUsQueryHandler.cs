using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.AboutUs.DTOs;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Enums;
using Stock_Exchange.Domain.Repositories.Interfaces;

namespace Stock_Exchange.Application.Features.AboutUs.Queries.GetAboutUs
{
    public class GetAboutUsQueryHandler : IRequestHandler<GetAboutUsQuery, Result<AboutUsDto>>
    {
        private readonly IAboutUsRepository _aboutUsRepository;
        private readonly ICurrentLanguageService _currentLanguageService;

        public GetAboutUsQueryHandler(
            IAboutUsRepository aboutUsRepository,
            ICurrentLanguageService currentLanguageService)
        {
            _aboutUsRepository = aboutUsRepository;
            _currentLanguageService = currentLanguageService;
        }

        public async Task<Result<AboutUsDto>> Handle(GetAboutUsQuery request, CancellationToken cancellationToken)
        {
            var aboutUs = await _aboutUsRepository
                .GetAllWithIncluding(a => a.IsActive, a => a.Features)
                .AsNoTracking()
                .Select(a => new AboutUsDto
                {
                    Id = a.Id,
                    StoryEn = a.StoryEn,
                    StoryAr = a.StoryAr,
                    MissionEn = a.MissionEn,
                    MissionAr = a.MissionAr,
                    VisionEn = a.VisionEn,
                    VisionAr = a.VisionAr,
                    Features = a.Features
                        .OrderBy(f => f.DisplayOrder)
                        .Select(f => new AboutUsFeatureDto
                        {
                            Id = f.Id,
                            TitleEn = f.TitleEn,
                            TitleAr = f.TitleAr,
                            DescriptionEn = f.DescriptionEn,
                            DescriptionAr = f.DescriptionAr,
                            Category = f.Category,
                            DisplayOrder = f.DisplayOrder
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (aboutUs is null)
                return Result<AboutUsDto>.Failure(LocalizationKeys.AboutUsMessages.AboutUsNotFound, StatusCodes.Status404NotFound);

            ApplyLanguageFilter(aboutUs, _currentLanguageService.Language);

            return Result<AboutUsDto>.Success(aboutUs);
        }

        private static void ApplyLanguageFilter(AboutUsDto aboutUs, Language language)
        {
            if (language == Language.en)
            {
                aboutUs.StoryAr = string.Empty;
                aboutUs.MissionAr = string.Empty;
                aboutUs.VisionAr = string.Empty;

                foreach (var feature in aboutUs.Features)
                {
                    feature.TitleAr = string.Empty;
                    feature.DescriptionAr = string.Empty;
                }

                return;
            }

            aboutUs.StoryEn = string.Empty;
            aboutUs.MissionEn = string.Empty;
            aboutUs.VisionEn = string.Empty;

            foreach (var feature in aboutUs.Features)
            {
                feature.TitleEn = string.Empty;
                feature.DescriptionEn = string.Empty;
            }
        }
    }
}
