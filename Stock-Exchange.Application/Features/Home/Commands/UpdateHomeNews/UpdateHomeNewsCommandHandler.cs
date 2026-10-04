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

namespace Stock_Exchange.Application.Features.Home.Commands.UpdateHomeNews
{
    public class UpdateHomeNewsCommandHandler : IRequestHandler<UpdateHomeNewsCommand, Result<List<HomeNewsDto>>>
    {
        private readonly INewsRepository _newsRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly ICurrentLanguageService _currentLanguageService;

        public UpdateHomeNewsCommandHandler(
            INewsRepository newsRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            ICurrentLanguageService currentLanguageService)
        {
            _newsRepository = newsRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _currentLanguageService = currentLanguageService;
        }

        public async Task<Result<List<HomeNewsDto>>> Handle(UpdateHomeNewsCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUserService.IsAuthenticated || _currentUserService.UserId == Guid.Empty)
                throw new UnAuthorizedException(LocalizationKeys.ExceptionMessages.Unauthorized);

            var existingNews = await _newsRepository.GetAllAsync(n => n.IsActive).ToListAsync(cancellationToken);
            var requestIds = request.Items.Where(i => i.Id.HasValue).Select(i => i.Id!.Value).ToHashSet();

            // Soft-delete items that were omitted
            foreach (var item in existingNews.Where(e => !requestIds.Contains(e.Id)))
            {
                _newsRepository.Delete(item);
            }

            // Upsert items from request
            foreach (var item in request.Items)
            {
                if (item.Id.HasValue && existingNews.FirstOrDefault(e => e.Id == item.Id.Value) is { } entity)
                {
                    entity.TitleEn = item.TitleEn.Trim();
                    entity.TitleAr = item.TitleAr.Trim();
                    entity.SummaryEn = item.SummaryEn?.Trim() ?? string.Empty;
                    entity.SummaryAr = item.SummaryAr?.Trim() ?? string.Empty;
                    entity.CategoryEn = item.CategoryEn?.Trim() ?? string.Empty;
                    entity.CategoryAr = item.CategoryAr?.Trim() ?? string.Empty;
                    entity.ImageUrl = string.IsNullOrWhiteSpace(item.ImageUrl) ? null : item.ImageUrl.Trim();
                    entity.PublishedAt = item.PublishedAt ?? DateTime.UtcNow;
                    entity.DisplayOrder = item.DisplayOrder;
                    entity.IsFeaturedOnHome = item.IsFeaturedOnHome;

                    _newsRepository.Update(entity);
                }
                else
                {
                    var newEntity = new Stock_Exchange.Domain.Entities.News
                    {
                        TitleEn = item.TitleEn.Trim(),
                        TitleAr = item.TitleAr.Trim(),
                        SummaryEn = item.SummaryEn?.Trim() ?? string.Empty,
                        SummaryAr = item.SummaryAr?.Trim() ?? string.Empty,
                        CategoryEn = item.CategoryEn?.Trim() ?? string.Empty,
                        CategoryAr = item.CategoryAr?.Trim() ?? string.Empty,
                        ImageUrl = string.IsNullOrWhiteSpace(item.ImageUrl) ? null : item.ImageUrl.Trim(),
                        PublishedAt = item.PublishedAt ?? DateTime.UtcNow,
                        DisplayOrder = item.DisplayOrder,
                        IsFeaturedOnHome = item.IsFeaturedOnHome
                    };

                    await _newsRepository.AddAsync(newEntity);
                }
            }

            await _unitOfWork.SaveChangesAsync();

            var updatedList = await _newsRepository
                .GetAllAsync(n => n.IsActive && n.IsFeaturedOnHome)
                .AsNoTracking()
                .OrderBy(n => n.DisplayOrder)
                .ThenByDescending(n => n.PublishedAt)
                .Select(n => new HomeNewsDto
                {
                    Id = n.Id,
                    TitleEn = n.TitleEn,
                    TitleAr = n.TitleAr,
                    SummaryEn = n.SummaryEn,
                    SummaryAr = n.SummaryAr,
                    CategoryEn = n.CategoryEn,
                    CategoryAr = n.CategoryAr,
                    ImageUrl = n.ImageUrl,
                    PublishedAt = n.PublishedAt
                })
                .ToListAsync(cancellationToken);

            var language = _currentLanguageService.Language;
            foreach (var item in updatedList)
            {
                item.ApplyLanguageFilter(language);
            }

            return Result<List<HomeNewsDto>>.Success(updatedList);
        }
    }
}
