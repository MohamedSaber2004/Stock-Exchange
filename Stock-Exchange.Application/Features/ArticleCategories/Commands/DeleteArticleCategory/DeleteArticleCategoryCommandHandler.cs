using MediatR;
using Stock_Exchange.Application.Common.Exceptions;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Repositories.Interfaces;
using Stock_Exchange.Domain.Repositories.Interfaces.Base;

namespace Stock_Exchange.Application.Features.ArticleCategories.Commands.DeleteArticleCategory
{
    public class DeleteArticleCategoryCommandHandler : IRequestHandler<DeleteArticleCategoryCommand, Result<bool>>
    {
        private readonly IArticleCategoryRepository _categoryRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public DeleteArticleCategoryCommandHandler(
            IArticleCategoryRepository categoryRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _categoryRepository = categoryRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result<bool>> Handle(DeleteArticleCategoryCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUserService.IsAuthenticated || _currentUserService.UserId == Guid.Empty)
                throw new UnAuthorizedException(LocalizationKeys.ExceptionMessages.Unauthorized);

            var category = await _categoryRepository.GetFirstAsync(
                c => c.Id == request.Id && !c.IsDeleted && c.IsActive,
                cancellationToken);

            if (category is null)
                throw new NotFoundException("Article category not found.");

            _categoryRepository.Delete(category);
            await _unitOfWork.SaveChangesAsync();

            return Result<bool>.Success(true);
        }
    }
}
