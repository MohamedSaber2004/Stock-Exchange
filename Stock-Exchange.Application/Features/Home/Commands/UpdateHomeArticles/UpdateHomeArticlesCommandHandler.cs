using MediatR;
using Microsoft.EntityFrameworkCore;
using Stock_Exchange.Application.Common.Exceptions;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.Home.DTOs;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Entities;
using Stock_Exchange.Domain.Repositories.Interfaces;
using Stock_Exchange.Domain.Repositories.Interfaces.Base;

namespace Stock_Exchange.Application.Features.Home.Commands.UpdateHomeArticles
{
    public class UpdateHomeArticlesCommandHandler : IRequestHandler<UpdateHomeArticlesCommand, Result<List<HomeArticleDto>>>
    {
        private readonly IArticleRepository _articleRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly ICurrentLanguageService _currentLanguageService;

        public UpdateHomeArticlesCommandHandler(
            IArticleRepository articleRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            ICurrentLanguageService currentLanguageService)
        {
            _articleRepository = articleRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _currentLanguageService = currentLanguageService;
        }

        public async Task<Result<List<HomeArticleDto>>> Handle(UpdateHomeArticlesCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUserService.IsAuthenticated || _currentUserService.UserId == Guid.Empty)
                throw new UnAuthorizedException(LocalizationKeys.ExceptionMessages.Unauthorized);

            var existingArticles = await _articleRepository.GetAllAsync(a => a.IsActive).ToListAsync(cancellationToken);
            var requestIds = request.Items.Where(i => i.Id.HasValue).Select(i => i.Id!.Value).ToHashSet();

            // Soft-delete items that were omitted
            foreach (var item in existingArticles.Where(e => !requestIds.Contains(e.Id)))
            {
                _articleRepository.Delete(item);
            }

            // Upsert items from request
            foreach (var item in request.Items)
            {
                if (item.Id.HasValue && existingArticles.FirstOrDefault(e => e.Id == item.Id.Value) is { } entity)
                {
                    entity.TitleEn = item.TitleEn.Trim();
                    entity.TitleAr = item.TitleAr.Trim();
                    entity.ExcerptEn = item.ExcerptEn?.Trim() ?? string.Empty;
                    entity.ExcerptAr = item.ExcerptAr?.Trim() ?? string.Empty;
                    entity.AuthorName = item.AuthorName?.Trim() ?? string.Empty;
                    entity.ReadMinutes = item.ReadMinutes;
                    entity.ImageUrl = string.IsNullOrWhiteSpace(item.ImageUrl) ? null : item.ImageUrl.Trim();
                    entity.PublishedAt = item.PublishedAt ?? DateTime.UtcNow;
                    entity.DisplayOrder = item.DisplayOrder;
                    entity.IsFeaturedOnHome = item.IsFeaturedOnHome;

                    _articleRepository.Update(entity);
                }
                else
                {
                    var newEntity = new Article
                    {
                        TitleEn = item.TitleEn.Trim(),
                        TitleAr = item.TitleAr.Trim(),
                        ExcerptEn = item.ExcerptEn?.Trim() ?? string.Empty,
                        ExcerptAr = item.ExcerptAr?.Trim() ?? string.Empty,
                        AuthorName = item.AuthorName?.Trim() ?? string.Empty,
                        ReadMinutes = item.ReadMinutes,
                        ImageUrl = string.IsNullOrWhiteSpace(item.ImageUrl) ? null : item.ImageUrl.Trim(),
                        PublishedAt = item.PublishedAt ?? DateTime.UtcNow,
                        DisplayOrder = item.DisplayOrder,
                        IsFeaturedOnHome = item.IsFeaturedOnHome
                    };

                    await _articleRepository.AddAsync(newEntity);
                }
            }

            await _unitOfWork.SaveChangesAsync();

            var updatedList = await _articleRepository
                .GetAllAsync(a => a.IsActive && a.IsFeaturedOnHome)
                .AsNoTracking()
                .OrderBy(a => a.DisplayOrder)
                .ThenByDescending(a => a.PublishedAt)
                .Select(a => new HomeArticleDto
                {
                    Id = a.Id,
                    TitleEn = a.TitleEn,
                    TitleAr = a.TitleAr,
                    ExcerptEn = a.ExcerptEn,
                    ExcerptAr = a.ExcerptAr,
                    ImageUrl = a.ImageUrl,
                    AuthorName = a.AuthorName,
                    ReadMinutes = a.ReadMinutes,
                    PublishedAt = a.PublishedAt
                })
                .ToListAsync(cancellationToken);

            var language = _currentLanguageService.Language;
            foreach (var item in updatedList)
            {
                item.ApplyLanguageFilter(language);
            }

            return Result<List<HomeArticleDto>>.Success(updatedList);
        }
    }
}
