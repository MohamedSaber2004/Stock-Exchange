using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.Articles.DTOs;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Repositories.Interfaces;

namespace Stock_Exchange.Application.Features.Articles.Queries.GetArticleById
{
    public class GetArticleByIdQueryHandler : IRequestHandler<GetArticleByIdQuery, Result<ArticleDto>>
    {
        private readonly IArticleRepository _articleRepository;
        private readonly ICurrentLanguageService _currentLanguageService;

        public GetArticleByIdQueryHandler(
            IArticleRepository articleRepository,
            ICurrentLanguageService currentLanguageService)
        {
            _articleRepository = articleRepository;
            _currentLanguageService = currentLanguageService;
        }

        public async Task<Result<ArticleDto>> Handle(GetArticleByIdQuery request, CancellationToken cancellationToken)
        {
            var article = await _articleRepository
                .GetAllAsync(a => a.Id == request.Id)
                .AsNoTracking()
                .Select(a => new ArticleDto
                {
                    Id = a.Id,
                    TitleEn = a.TitleEn,
                    TitleAr = a.TitleAr,
                    ExcerptEn = a.ExcerptEn,
                    ExcerptAr = a.ExcerptAr,
                    ImageUrl = a.ImageUrl,
                    AuthorName = a.AuthorName,
                    PublishedAt = a.PublishedAt,
                    IsFeaturedOnHome = a.IsFeaturedOnHome,
                    DisplayOrder = a.DisplayOrder,
                    IsActive = a.IsActive,
                    CreatedAt = a.CreatedAt
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (article is null)
                return Result<ArticleDto>.Failure(LocalizationKeys.ArticleMessages.ArticleNotFound, StatusCodes.Status404NotFound);

            if (request.ApplyLanguageFilter ?? true)
            {
                article.ApplyLanguageFilter(_currentLanguageService.Language);
            }

            return Result<ArticleDto>.Success(article);
        }
    }
}
