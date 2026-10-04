using MediatR;
using Stock_Exchange.Application.Common.Exceptions;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.VideoCategories.DTOs;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Repositories.Interfaces;
using Stock_Exchange.Domain.Repositories.Interfaces.Base;

namespace Stock_Exchange.Application.Features.VideoCategories.Commands.UpdateVideoCategory
{
    public class UpdateVideoCategoryCommandHandler : IRequestHandler<UpdateVideoCategoryCommand, Result<VideoCategoryDto>>
    {
        private readonly IVideoCategoryRepository _categoryRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public UpdateVideoCategoryCommandHandler(
            IVideoCategoryRepository categoryRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _categoryRepository = categoryRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result<VideoCategoryDto>> Handle(UpdateVideoCategoryCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUserService.IsAuthenticated || _currentUserService.UserId == Guid.Empty)
                throw new UnAuthorizedException(LocalizationKeys.ExceptionMessages.Unauthorized);

            var category = await _categoryRepository.GetFirstAsync(
                c => c.Id == request.Id && !c.IsDeleted && c.IsActive,
                cancellationToken);

            if (category is null)
                throw new NotFoundException("Video category not found.");

            category.CategoryArName = request.CategoryArName.Trim();
            category.CategoryEnName = request.CategoryEnName.Trim();

            _categoryRepository.Update(category);
            await _unitOfWork.SaveChangesAsync();

            return Result<VideoCategoryDto>.Success(new VideoCategoryDto
            {
                Id = category.Id,
                CategoryArName = category.CategoryArName,
                CategoryEnName = category.CategoryEnName,
                IsActive = category.IsActive,
                CreatedAt = category.CreatedAt
            });
        }
    }
}
