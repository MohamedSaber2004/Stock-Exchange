using MediatR;
using Microsoft.EntityFrameworkCore;
using Stock_Exchange.Application.Common.Exceptions;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.AboutUs.DTOs;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Entities;
using Stock_Exchange.Domain.Repositories.Interfaces;
using Stock_Exchange.Domain.Repositories.Interfaces.Base;
using AboutUsEntity = Stock_Exchange.Domain.Entities.AboutUs;

namespace Stock_Exchange.Application.Features.AboutUs.Commands.UpdateAboutUs
{
    public class UpdateAboutUsCommandHandler : IRequestHandler<UpdateAboutUsCommand, Result<AboutUsDto>>
    {
        private readonly IAboutUsRepository _aboutUsRepository;
        private readonly IAboutUsFeatureRepository _aboutUsFeatureRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public UpdateAboutUsCommandHandler(
            IAboutUsRepository aboutUsRepository,
            IAboutUsFeatureRepository aboutUsFeatureRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _aboutUsRepository = aboutUsRepository;
            _aboutUsFeatureRepository = aboutUsFeatureRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result<AboutUsDto>> Handle(UpdateAboutUsCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUserService.IsAuthenticated || _currentUserService.UserId == Guid.Empty)
                throw new UnAuthorizedException(LocalizationKeys.ExceptionMessages.Unauthorized);

            var aboutUs = await _aboutUsRepository.GetFirstAsync(a => a.IsActive, cancellationToken);

            if (aboutUs is null)
            {
                aboutUs = new AboutUsEntity
                {
                    StoryEn = request.StoryEn?.Trim() ?? string.Empty,
                    StoryAr = request.StoryAr?.Trim() ?? string.Empty,
                    MissionEn = request.MissionEn?.Trim() ?? string.Empty,
                    MissionAr = request.MissionAr?.Trim() ?? string.Empty,
                    VisionEn = request.VisionEn?.Trim() ?? string.Empty,
                    VisionAr = request.VisionAr?.Trim() ?? string.Empty,
                    SupportEmail = string.IsNullOrWhiteSpace(request.SupportEmail) ? null : request.SupportEmail.Trim()
                };

                await _aboutUsRepository.AddAsync(aboutUs);
            }
            else
            {
                if (request.StoryEn is not null)
                    aboutUs.StoryEn = request.StoryEn.Trim();

                if (request.StoryAr is not null)
                    aboutUs.StoryAr = request.StoryAr.Trim();

                if (request.MissionEn is not null)
                    aboutUs.MissionEn = request.MissionEn.Trim();

                if (request.MissionAr is not null)
                    aboutUs.MissionAr = request.MissionAr.Trim();

                if (request.VisionEn is not null)
                    aboutUs.VisionEn = request.VisionEn.Trim();

                if (request.VisionAr is not null)
                    aboutUs.VisionAr = request.VisionAr.Trim();

                if (request.SupportEmail is not null)
                    aboutUs.SupportEmail = string.IsNullOrWhiteSpace(request.SupportEmail) ? null : request.SupportEmail.Trim();

                _aboutUsRepository.Update(aboutUs);
            }

            if (request.Features is not null)
            {
                var existingFeatures = await _aboutUsFeatureRepository
                    .GetAllAsync(f => f.AboutUsId == aboutUs.Id)
                    .ToListAsync(cancellationToken);

                foreach (var existingFeature in existingFeatures)
                    _aboutUsFeatureRepository.Delete(existingFeature);

                var newFeatures = request.Features
                    .Where(f => !string.IsNullOrWhiteSpace(f.TitleEn) || !string.IsNullOrWhiteSpace(f.TitleAr))
                    .Select((f, index) => new AboutUsFeature
                    {
                        AboutUsId = aboutUs.Id,
                        TitleEn = f.TitleEn?.Trim() ?? string.Empty,
                        TitleAr = f.TitleAr?.Trim() ?? string.Empty,
                        DescriptionEn = f.DescriptionEn?.Trim() ?? string.Empty,
                        DescriptionAr = f.DescriptionAr?.Trim() ?? string.Empty,
                        Category = f.Category?.Trim() ?? string.Empty,
                        DisplayOrder = index
                    })
                    .ToList();

                if (newFeatures.Count > 0)
                    await _aboutUsFeatureRepository.AddRangeAsync(newFeatures);
            }

            await _unitOfWork.SaveChangesAsync();

            var features = await _aboutUsFeatureRepository
                .GetAllAsync(f => f.AboutUsId == aboutUs.Id)
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
                .ToListAsync(cancellationToken);

            return Result<AboutUsDto>.Success(new AboutUsDto
            {
                Id = aboutUs.Id,
                StoryEn = aboutUs.StoryEn,
                StoryAr = aboutUs.StoryAr,
                MissionEn = aboutUs.MissionEn,
                MissionAr = aboutUs.MissionAr,
                VisionEn = aboutUs.VisionEn,
                VisionAr = aboutUs.VisionAr,
                SupportEmail = aboutUs.SupportEmail,
                Features = features
            });
        }
    }
}
