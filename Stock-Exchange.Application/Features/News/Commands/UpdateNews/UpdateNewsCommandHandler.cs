using MediatR;
using Stock_Exchange.Application.Common.Exceptions;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.News.DTOs;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Repositories.Interfaces;
using Stock_Exchange.Domain.Repositories.Interfaces.Base;

namespace Stock_Exchange.Application.Features.News.Commands.UpdateNews
{
    public class UpdateNewsCommandHandler : IRequestHandler<UpdateNewsCommand, Result<NewsDto>>
    {
        private readonly INewsRepository _newsRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public UpdateNewsCommandHandler(
            INewsRepository newsRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _newsRepository = newsRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result<NewsDto>> Handle(UpdateNewsCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUserService.IsAuthenticated || _currentUserService.UserId == Guid.Empty)
                throw new UnAuthorizedException(LocalizationKeys.ExceptionMessages.Unauthorized);

            var news = await _newsRepository.GetFirstAsync(n => n.Id == request.Id, cancellationToken);
            if (news is null)
                throw new NotFoundException("News item not found");

            news.TitleEn = request.TitleEn.Trim();
            news.TitleAr = request.TitleAr.Trim();
            news.SummaryEn = request.SummaryEn?.Trim() ?? string.Empty;
            news.SummaryAr = request.SummaryAr?.Trim() ?? string.Empty;
            if (request.ContentEn != null) news.ContentEn = request.ContentEn.Trim();
            if (request.ContentAr != null) news.ContentAr = request.ContentAr.Trim();
            news.CategoryEn = request.CategoryEn?.Trim() ?? string.Empty;
            news.CategoryAr = request.CategoryAr?.Trim() ?? string.Empty;
            news.ImageUrl = string.IsNullOrWhiteSpace(request.ImageUrl) ? null : request.ImageUrl.Trim();
            if (request.PublishedAt.HasValue) news.PublishedAt = request.PublishedAt.Value;
            news.DisplayOrder = request.DisplayOrder;
            news.IsFeaturedOnHome = request.IsFeaturedOnHome;

            if (news.IsActive != request.IsActive)
            {
                news.SetActiveState(request.IsActive, _currentUserService.UserId.ToString());
            }

            _newsRepository.Update(news);
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
