using MediatR;
using Stock_Exchange.Application.Common.Exceptions;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.Articles.DTOs;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Entities;
using Stock_Exchange.Domain.Repositories.Interfaces;
using Stock_Exchange.Domain.Repositories.Interfaces.Base;

namespace Stock_Exchange.Application.Features.Articles.Commands.AddArticle
{
    public class AddArticleCommandHandler : IRequestHandler<AddArticleCommand, Result<ArticleDto>>
    {
        private readonly IArticleRepository _articleRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public AddArticleCommandHandler(
            IArticleRepository articleRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _articleRepository = articleRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result<ArticleDto>> Handle(AddArticleCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUserService.IsAuthenticated || _currentUserService.UserId == Guid.Empty)
                throw new UnAuthorizedException(LocalizationKeys.ExceptionMessages.Unauthorized);

            var article = new Article
            {
                TitleEn = request.TitleEn.Trim(),
                TitleAr = request.TitleAr.Trim(),
                ExcerptEn = request.ExcerptEn.Trim(),
                ExcerptAr = request.ExcerptAr.Trim(),
                ContentEn = request.ContentEn?.Trim() ?? string.Empty,
                ContentAr = request.ContentAr?.Trim() ?? string.Empty,
                ImageUrl = string.IsNullOrWhiteSpace(request.ImageUrl) ? null : request.ImageUrl.Trim(),
                AuthorName = request.AuthorName.Trim(),
                ReadMinutes = request.ReadMinutes ?? 5,
                PublishedAt = request.PublishedAt ?? DateTime.UtcNow,
                IsFeaturedOnHome = request.IsFeaturedOnHome,
                DisplayOrder = request.DisplayOrder,
                CategoryId = request.CategoryId
            };

            if (!request.IsActive)
            {
                article.SetActiveState(false, _currentUserService.UserId.ToString());
            }

            await _articleRepository.AddAsync(article);
            await _unitOfWork.SaveChangesAsync();

            return Result<ArticleDto>.Success(new ArticleDto
            {
                Id = article.Id,
                TitleEn = article.TitleEn,
                TitleAr = article.TitleAr,
                ExcerptEn = article.ExcerptEn,
                ExcerptAr = article.ExcerptAr,
                ContentEn = article.ContentEn,
                ContentAr = article.ContentAr,
                ImageUrl = article.ImageUrl,
                AuthorName = article.AuthorName,
                ReadMinutes = article.ReadMinutes,
                PublishedAt = article.PublishedAt,
                IsFeaturedOnHome = article.IsFeaturedOnHome,
                DisplayOrder = article.DisplayOrder,
                IsActive = article.IsActive,
                CreatedAt = article.CreatedAt,
                CategoryId = article.CategoryId,
                ArticleCategoryId = article.CategoryId
            });
        }
    }
}
