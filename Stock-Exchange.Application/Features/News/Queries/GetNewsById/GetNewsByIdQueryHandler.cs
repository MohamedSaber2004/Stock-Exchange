using MediatR;
using Microsoft.EntityFrameworkCore;
using Stock_Exchange.Application.Common.Exceptions;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.News.DTOs;
using Stock_Exchange.Domain.Repositories.Interfaces;

namespace Stock_Exchange.Application.Features.News.Queries.GetNewsById
{
    public class GetNewsByIdQueryHandler : IRequestHandler<GetNewsByIdQuery, Result<NewsDto>>
    {
        private readonly INewsRepository _newsRepository;
        private readonly ICurrentLanguageService _currentLanguageService;

        public GetNewsByIdQueryHandler(
            INewsRepository newsRepository,
            ICurrentLanguageService currentLanguageService)
        {
            _newsRepository = newsRepository;
            _currentLanguageService = currentLanguageService;
        }

        public async Task<Result<NewsDto>> Handle(GetNewsByIdQuery request, CancellationToken cancellationToken)
        {
            var news = await _newsRepository
                .GetAllAsync(n => n.Id == request.Id)
                .AsNoTracking()
                .Select(n => new NewsDto
                {
                    Id = n.Id,
                    TitleEn = n.TitleEn,
                    TitleAr = n.TitleAr,
                    SummaryEn = n.SummaryEn,
                    SummaryAr = n.SummaryAr,
                    ImageUrl = n.ImageUrl,
                    CategoryEn = n.CategoryEn,
                    CategoryAr = n.CategoryAr,
                    PublishedAt = n.PublishedAt,
                    DisplayOrder = n.DisplayOrder,
                    IsFeaturedOnHome = n.IsFeaturedOnHome,
                    IsActive = n.IsActive,
                    CreatedAt = n.CreatedAt
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (news is null)
                throw new NotFoundException("News item not found");

            var apply = request.ApplyLanguageFilter ?? true;
            if (apply)
            {
                news.ApplyLanguageFilter(_currentLanguageService.Language);
            }

            return Result<NewsDto>.Success(news);
        }
    }
}
