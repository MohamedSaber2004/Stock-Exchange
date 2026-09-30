using MediatR;
using Stock_Exchange.Application.Common.Exceptions;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.HelpCenterCategories.DTOs;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Repositories.Interfaces;
using Stock_Exchange.Domain.Repositories.Interfaces.Base;

namespace Stock_Exchange.Application.Features.HelpCenterCategories.Commands.UpdateHelpCenterCategory
{
    public class UpdateHelpCenterCategoryCommandHandler : IRequestHandler<UpdateHelpCenterCategoryCommand, Result<HelpCenterCategoryDto>>
    {
        private readonly IHelpCenterCategoryRepository _categoryRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public UpdateHelpCenterCategoryCommandHandler(
            IHelpCenterCategoryRepository categoryRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _categoryRepository = categoryRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result<HelpCenterCategoryDto>> Handle(UpdateHelpCenterCategoryCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUserService.IsAuthenticated || _currentUserService.UserId == Guid.Empty)
                throw new UnAuthorizedException(LocalizationKeys.ExceptionMessages.Unauthorized);

            var category = await _categoryRepository.GetFirstAsync(
                c => c.Id == request.Id && !c.IsDeleted && c.IsActive,
                cancellationToken);

            if (category is null)
                throw new NotFoundException(LocalizationKeys.HelpCenterMessages.CategoryNotFound);

            category.TitleEn = request.TitleEn.Trim();
            category.TitleAr = request.TitleAr.Trim();

            _categoryRepository.Update(category);
            await _unitOfWork.SaveChangesAsync();

            return Result<HelpCenterCategoryDto>.Success(new HelpCenterCategoryDto
            {
                Id = category.Id,
                TitleEn = category.TitleEn,
                TitleAr = category.TitleAr
            });
        }
    }
}
