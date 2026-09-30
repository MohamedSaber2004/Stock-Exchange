using MediatR;
using Stock_Exchange.Application.Common.Exceptions;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.HelpCenterCategories.DTOs;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Entities;
using Stock_Exchange.Domain.Repositories.Interfaces;
using Stock_Exchange.Domain.Repositories.Interfaces.Base;

namespace Stock_Exchange.Application.Features.HelpCenterCategories.Commands.AddHelpCenterCategory
{
    public class AddHelpCenterCategoryCommandHandler : IRequestHandler<AddHelpCenterCategoryCommand, Result<HelpCenterCategoryDto>>
    {
        private readonly IHelpCenterCategoryRepository _categoryRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public AddHelpCenterCategoryCommandHandler(
            IHelpCenterCategoryRepository categoryRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _categoryRepository = categoryRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result<HelpCenterCategoryDto>> Handle(AddHelpCenterCategoryCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUserService.IsAuthenticated || _currentUserService.UserId == Guid.Empty)
                throw new UnAuthorizedException(LocalizationKeys.ExceptionMessages.Unauthorized);

            var category = new HelpCenterCategory
            {
                TitleEn = request.TitleEn.Trim(),
                TitleAr = request.TitleAr.Trim()
            };

            await _categoryRepository.AddAsync(category);
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
