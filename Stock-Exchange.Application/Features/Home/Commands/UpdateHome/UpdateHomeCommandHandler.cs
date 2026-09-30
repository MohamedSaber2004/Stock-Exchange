using MediatR;
using Stock_Exchange.Application.Common.Exceptions;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.Home.DTOs;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Repositories.Interfaces;
using Stock_Exchange.Domain.Repositories.Interfaces.Base;
using HomeEntity = Stock_Exchange.Domain.Entities.Home;

namespace Stock_Exchange.Application.Features.Home.Commands.UpdateHome
{
    public class UpdateHomeCommandHandler : IRequestHandler<UpdateHomeCommand, Result<HomeHeroDto>>
    {
        private readonly IHomeRepository _homeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly ICurrentLanguageService _currentLanguageService;

        public UpdateHomeCommandHandler(
            IHomeRepository homeRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            ICurrentLanguageService currentLanguageService)
        {
            _homeRepository = homeRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _currentLanguageService = currentLanguageService;
        }

        public async Task<Result<HomeHeroDto>> Handle(UpdateHomeCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUserService.IsAuthenticated || _currentUserService.UserId == Guid.Empty)
                throw new UnAuthorizedException(LocalizationKeys.ExceptionMessages.Unauthorized);

            var home = await _homeRepository.GetFirstAsync(h => h.IsActive, cancellationToken);

            if (home is null)
            {
                home = new HomeEntity
                {
                    HeroTitleEn = request.HeroTitleEn?.Trim() ?? string.Empty,
                    HeroTitleAr = request.HeroTitleAr?.Trim() ?? string.Empty,
                    HeroSubtitleEn = request.HeroSubtitleEn?.Trim() ?? string.Empty,
                    HeroSubtitleAr = request.HeroSubtitleAr?.Trim() ?? string.Empty,
                    HeroImageUrl = string.IsNullOrWhiteSpace(request.HeroImageUrl) ? null : request.HeroImageUrl.Trim()
                };

                await _homeRepository.AddAsync(home);
            }
            else
            {
                if (request.HeroTitleEn is not null)
                    home.HeroTitleEn = request.HeroTitleEn.Trim();

                if (request.HeroTitleAr is not null)
                    home.HeroTitleAr = request.HeroTitleAr.Trim();

                if (request.HeroSubtitleEn is not null)
                    home.HeroSubtitleEn = request.HeroSubtitleEn.Trim();

                if (request.HeroSubtitleAr is not null)
                    home.HeroSubtitleAr = request.HeroSubtitleAr.Trim();

                if (request.HeroImageUrl is not null)
                    home.HeroImageUrl = string.IsNullOrWhiteSpace(request.HeroImageUrl) ? null : request.HeroImageUrl.Trim();

                _homeRepository.Update(home);
            }

            await _unitOfWork.SaveChangesAsync();

            var heroDto = new HomeHeroDto
            {
                TitleEn = home.HeroTitleEn,
                TitleAr = home.HeroTitleAr,
                SubtitleEn = home.HeroSubtitleEn,
                SubtitleAr = home.HeroSubtitleAr,
                ImageUrl = home.HeroImageUrl
            };

            heroDto.ApplyLanguageFilter(_currentLanguageService.Language);

            return Result<HomeHeroDto>.Success(heroDto);
        }
    }
}
