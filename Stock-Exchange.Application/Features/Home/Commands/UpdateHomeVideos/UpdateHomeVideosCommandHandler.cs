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

namespace Stock_Exchange.Application.Features.Home.Commands.UpdateHomeVideos
{
    public class UpdateHomeVideosCommandHandler : IRequestHandler<UpdateHomeVideosCommand, Result<List<HomeVideoDto>>>
    {
        private readonly IVideoRepository _videoRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly ICurrentLanguageService _currentLanguageService;

        public UpdateHomeVideosCommandHandler(
            IVideoRepository videoRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            ICurrentLanguageService currentLanguageService)
        {
            _videoRepository = videoRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _currentLanguageService = currentLanguageService;
        }

        public async Task<Result<List<HomeVideoDto>>> Handle(UpdateHomeVideosCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUserService.IsAuthenticated || _currentUserService.UserId == Guid.Empty)
                throw new UnAuthorizedException(LocalizationKeys.ExceptionMessages.Unauthorized);

            var existingVideos = await _videoRepository.GetAllAsync(v => v.IsActive).ToListAsync(cancellationToken);
            var requestIds = request.Items.Where(i => i.Id.HasValue).Select(i => i.Id!.Value).ToHashSet();

            // Soft-delete items that were omitted
            foreach (var item in existingVideos.Where(e => !requestIds.Contains(e.Id)))
            {
                _videoRepository.Delete(item);
            }

            // Upsert items from request
            foreach (var item in request.Items)
            {
                if (item.Id.HasValue && existingVideos.FirstOrDefault(e => e.Id == item.Id.Value) is { } entity)
                {
                    entity.TitleEn = item.TitleEn.Trim();
                    entity.TitleAr = item.TitleAr.Trim();
                    entity.ThumbnailUrl = string.IsNullOrWhiteSpace(item.ThumbnailUrl) ? null : item.ThumbnailUrl.Trim();
                    entity.VideoUrl = string.IsNullOrWhiteSpace(item.VideoUrl) ? null : item.VideoUrl.Trim();
                    entity.InstructorName = item.InstructorName?.Trim() ?? string.Empty;
                    entity.CategoryEn = item.CategoryEn?.Trim() ?? string.Empty;
                    entity.CategoryAr = item.CategoryAr?.Trim() ?? string.Empty;
                    entity.IsPreviewable = item.IsPreviewable;
                    entity.IsFeaturedOnHome = item.IsFeaturedOnHome;
                    entity.DisplayOrder = item.DisplayOrder;

                    _videoRepository.Update(entity);
                }
                else
                {
                    var newEntity = new Video
                    {
                        TitleEn = item.TitleEn.Trim(),
                        TitleAr = item.TitleAr.Trim(),
                        ThumbnailUrl = string.IsNullOrWhiteSpace(item.ThumbnailUrl) ? null : item.ThumbnailUrl.Trim(),
                        VideoUrl = string.IsNullOrWhiteSpace(item.VideoUrl) ? null : item.VideoUrl.Trim(),
                        InstructorName = item.InstructorName?.Trim() ?? string.Empty,
                        CategoryEn = item.CategoryEn?.Trim() ?? string.Empty,
                        CategoryAr = item.CategoryAr?.Trim() ?? string.Empty,
                        IsPreviewable = item.IsPreviewable,
                        IsFeaturedOnHome = item.IsFeaturedOnHome,
                        DisplayOrder = item.DisplayOrder
                    };

                    await _videoRepository.AddAsync(newEntity);
                }
            }

            await _unitOfWork.SaveChangesAsync();

            var updatedList = await _videoRepository
                .GetAllAsync(v => v.IsActive && v.IsFeaturedOnHome)
                .AsNoTracking()
                .OrderBy(v => v.DisplayOrder)
                .Select(v => new HomeVideoDto
                {
                    Id = v.Id,
                    TitleEn = v.TitleEn,
                    TitleAr = v.TitleAr,
                    ThumbnailUrl = v.ThumbnailUrl,
                    VideoUrl = v.VideoUrl,
                    InstructorName = v.InstructorName,
                    CategoryEn = v.CategoryEn,
                    CategoryAr = v.CategoryAr,
                    IsPreviewable = v.IsPreviewable,
                    IsFeaturedOnHome = v.IsFeaturedOnHome
                })
                .ToListAsync(cancellationToken);

            var language = _currentLanguageService.Language;
            foreach (var item in updatedList)
            {
                item.ApplyLanguageFilter(language);
            }

            return Result<List<HomeVideoDto>>.Success(updatedList);
        }
    }
}
