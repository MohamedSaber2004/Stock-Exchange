using MediatR;
using Stock_Exchange.Application.Common.Exceptions;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.Videos.DTOs;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Entities;
using Stock_Exchange.Domain.Repositories.Interfaces;
using Stock_Exchange.Domain.Repositories.Interfaces.Base;

namespace Stock_Exchange.Application.Features.Videos.Commands.AddVideo
{
    public class AddVideoCommandHandler : IRequestHandler<AddVideoCommand, Result<VideoDto>>
    {
        private readonly IVideoRepository _videoRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public AddVideoCommandHandler(
            IVideoRepository videoRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _videoRepository = videoRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result<VideoDto>> Handle(AddVideoCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUserService.IsAuthenticated || _currentUserService.UserId == Guid.Empty)
                throw new UnAuthorizedException(LocalizationKeys.ExceptionMessages.Unauthorized);

            var video = new Video
            {
                TitleEn = request.TitleEn.Trim(),
                TitleAr = request.TitleAr.Trim(),
                ThumbnailUrl = string.IsNullOrWhiteSpace(request.ThumbnailUrl) ? null : request.ThumbnailUrl.Trim(),
                VideoUrl = string.IsNullOrWhiteSpace(request.VideoUrl) ? null : request.VideoUrl.Trim(),
                DurationSeconds = request.DurationSeconds,
                InstructorName = request.InstructorName.Trim(),
                CategoryEn = request.CategoryEn.Trim(),
                CategoryAr = request.CategoryAr.Trim(),
                IsPreviewable = request.IsPreviewable,
                IsFeaturedOnHome = request.IsFeaturedOnHome,
                DisplayOrder = request.DisplayOrder,
                CategoryId = request.CategoryId
            };

            if (!request.IsActive)
            {
                video.SetActiveState(false, _currentUserService.UserId.ToString());
            }

            await _videoRepository.AddAsync(video);
            await _unitOfWork.SaveChangesAsync();

            return Result<VideoDto>.Success(new VideoDto
            {
                Id = video.Id,
                TitleEn = video.TitleEn,
                TitleAr = video.TitleAr,
                ThumbnailUrl = video.ThumbnailUrl,
                VideoUrl = video.VideoUrl,
                DurationSeconds = video.DurationSeconds,
                InstructorName = video.InstructorName,
                CategoryEn = video.CategoryEn,
                CategoryAr = video.CategoryAr,
                IsPreviewable = video.IsPreviewable,
                IsFeaturedOnHome = video.IsFeaturedOnHome,
                DisplayOrder = video.DisplayOrder,
                IsActive = video.IsActive,
                CreatedAt = video.CreatedAt,
                CategoryId = video.CategoryId,
                VideoCategoryId = video.CategoryId
            });
        }
    }
}
