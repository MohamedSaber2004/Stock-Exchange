using MediatR;
using Stock_Exchange.Application.Common.Exceptions;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.Articles.DTOs;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Repositories.Interfaces;
using Stock_Exchange.Domain.Repositories.Interfaces.Base;

namespace Stock_Exchange.Application.Features.Articles.Commands.UpdateArticle
{
    public class UpdateArticleCommandHandler : IRequestHandler<UpdateArticleCommand, Result<ArticleDto>>
    {
        private readonly IArticleRepository _articleRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public UpdateArticleCommandHandler(
            IArticleRepository articleRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _articleRepository = articleRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result<ArticleDto>> Handle(UpdateArticleCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUserService.IsAuthenticated || _currentUserService.UserId == Guid.Empty)
                throw new UnAuthorizedException(LocalizationKeys.ExceptionMessages.Unauthorized);

            var article = await _articleRepository.GetFirstAsync(
                a => a.Id == request.Id,
                cancellationToken);

            if (article is null)
                throw new NotFoundException(LocalizationKeys.ArticleMessages.ArticleNotFound);

            article.TitleEn = request.TitleEn.Trim();
            article.TitleAr = request.TitleAr.Trim();
            article.ExcerptEn = request.ExcerptEn.Trim();
            article.ExcerptAr = request.ExcerptAr.Trim();
            if (request.ContentEn != null) article.ContentEn = request.ContentEn.Trim();
            if (request.ContentAr != null) article.ContentAr = request.ContentAr.Trim();
            article.ImageUrl = string.IsNullOrWhiteSpace(request.ImageUrl) ? null : request.ImageUrl.Trim();
            article.AuthorName = request.AuthorName.Trim();
            if (request.ReadMinutes.HasValue) article.ReadMinutes = request.ReadMinutes.Value;
            if (request.PublishedAt.HasValue)
            {
                article.PublishedAt = request.PublishedAt.Value;
            }
            article.IsFeaturedOnHome = request.IsFeaturedOnHome;
            article.DisplayOrder = request.DisplayOrder;
            article.CategoryId = request.CategoryId;
            if (article.IsActive != request.IsActive)
            {
                article.SetActiveState(request.IsActive, _currentUserService.UserId.ToString());
            }

            _articleRepository.Update(article);
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
