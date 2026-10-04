using MediatR;
using Stock_Exchange.Application.Common.Exceptions;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.VideoCategories.DTOs;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Entities;
using Stock_Exchange.Domain.Repositories.Interfaces;
using Stock_Exchange.Domain.Repositories.Interfaces.Base;

namespace Stock_Exchange.Application.Features.VideoCategories.Commands.AddVideoCategory
{
    public class AddVideoCategoryCommandHandler : IRequestHandler<AddVideoCategoryCommand, Result<VideoCategoryDto>>
    {
        private readonly IVideoCategoryRepository _categoryRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public AddVideoCategoryCommandHandler(
            IVideoCategoryRepository categoryRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _categoryRepository = categoryRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result<VideoCategoryDto>> Handle(AddVideoCategoryCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUserService.IsAuthenticated || _currentUserService.UserId == Guid.Empty)
                throw new UnAuthorizedException(LocalizationKeys.ExceptionMessages.Unauthorized);

            var category = new VideoCategory
            {
                CategoryArName = request.CategoryArName.Trim(),
                CategoryEnName = request.CategoryEnName.Trim()
            };

            await _categoryRepository.AddAsync(category);
            await _unitOfWork.SaveChangesAsync();

            return Result<VideoCategoryDto>.Success(new VideoCategoryDto
            {
                Id = category.Id,
                CategoryArName = category.CategoryArName,
                CategoryEnName = category.CategoryEnName,
                VideosCount = 0,
                IsActive = category.IsActive,
                CreatedAt = category.CreatedAt
            });
        }
    }
}
