using MediatR;
using Stock_Exchange.Application.Common.Exceptions;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.News.DTOs;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Entities;
using Stock_Exchange.Domain.Repositories.Interfaces;
using Stock_Exchange.Domain.Repositories.Interfaces.Base;

namespace Stock_Exchange.Application.Features.News.Commands.AddNews
{
    public class AddNewsCommandHandler : IRequestHandler<AddNewsCommand, Result<NewsDto>>
    {
        private readonly INewsRepository _newsRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public AddNewsCommandHandler(
            INewsRepository newsRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _newsRepository = newsRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result<NewsDto>> Handle(AddNewsCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUserService.IsAuthenticated || _currentUserService.UserId == Guid.Empty)
                throw new UnAuthorizedException(LocalizationKeys.ExceptionMessages.Unauthorized);

            var news = new Domain.Entities.News
            {
                TitleEn = request.TitleEn.Trim(),
                TitleAr = request.TitleAr.Trim(),
                SummaryEn = request.SummaryEn?.Trim() ?? string.Empty,
                SummaryAr = request.SummaryAr?.Trim() ?? string.Empty,
                ContentEn = request.ContentEn?.Trim() ?? string.Empty,
                ContentAr = request.ContentAr?.Trim() ?? string.Empty,
                CategoryEn = request.CategoryEn?.Trim() ?? string.Empty,
                CategoryAr = request.CategoryAr?.Trim() ?? string.Empty,
                ImageUrl = string.IsNullOrWhiteSpace(request.ImageUrl) ? null : request.ImageUrl.Trim(),
                PublishedAt = request.PublishedAt ?? DateTime.UtcNow,
                DisplayOrder = request.DisplayOrder,
                IsFeaturedOnHome = request.IsFeaturedOnHome
            };

            if (!request.IsActive)
            {
                news.SetActiveState(false, _currentUserService.UserId.ToString());
            }

            await _newsRepository.AddAsync(news);
            await _unitOfWork.SaveChangesAsync();

            return Result<NewsDto>.Success(new NewsDto
            {
                Id = news.Id,
                TitleEn = news.TitleEn,
                TitleAr = news.TitleAr,
                SummaryEn = news.SummaryEn,
                SummaryAr = news.SummaryAr,
                ContentEn = news.ContentEn,
                ContentAr = news.ContentAr,
                CategoryEn = news.CategoryEn,
                CategoryAr = news.CategoryAr,
                ImageUrl = news.ImageUrl,
                PublishedAt = news.PublishedAt,
                DisplayOrder = news.DisplayOrder,
                IsFeaturedOnHome = news.IsFeaturedOnHome,
                IsActive = news.IsActive,
                CreatedAt = news.CreatedAt
            });
        }
    }
}
