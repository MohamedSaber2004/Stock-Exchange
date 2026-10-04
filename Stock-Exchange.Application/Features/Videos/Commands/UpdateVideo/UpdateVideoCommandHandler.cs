using MediatR;
using Stock_Exchange.Application.Common.Exceptions;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.Videos.DTOs;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Repositories.Interfaces;
using Stock_Exchange.Domain.Repositories.Interfaces.Base;

namespace Stock_Exchange.Application.Features.Videos.Commands.UpdateVideo
{
    public class UpdateVideoCommandHandler : IRequestHandler<UpdateVideoCommand, Result<VideoDto>>
    {
        private readonly IVideoRepository _videoRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public UpdateVideoCommandHandler(
            IVideoRepository videoRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _videoRepository = videoRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result<VideoDto>> Handle(UpdateVideoCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUserService.IsAuthenticated || _currentUserService.UserId == Guid.Empty)
                throw new UnAuthorizedException(LocalizationKeys.ExceptionMessages.Unauthorized);

            var video = await _videoRepository.GetFirstAsync(
                v => v.Id == request.Id,
                cancellationToken);

            if (video is null)
                throw new NotFoundException(LocalizationKeys.VideoMessages.VideoNotFound);

            video.TitleEn = request.TitleEn.Trim();
            video.TitleAr = request.TitleAr.Trim();
            video.ThumbnailUrl = string.IsNullOrWhiteSpace(request.ThumbnailUrl) ? null : request.ThumbnailUrl.Trim();
            video.VideoUrl = string.IsNullOrWhiteSpace(request.VideoUrl) ? null : request.VideoUrl.Trim();
            video.DurationSeconds = request.DurationSeconds;
            video.InstructorName = request.InstructorName.Trim();
            video.CategoryEn = request.CategoryEn.Trim();
            video.CategoryAr = request.CategoryAr.Trim();
            video.IsPreviewable = request.IsPreviewable;
            video.IsFeaturedOnHome = request.IsFeaturedOnHome;
            video.DisplayOrder = request.DisplayOrder;
            video.CategoryId = request.CategoryId;

            if (video.IsActive != request.IsActive)
            {
                video.SetActiveState(request.IsActive, _currentUserService.UserId.ToString());
            }

            _videoRepository.Update(video);
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
