using MediatR;
using Stock_Exchange.Application.Common.Exceptions;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.ArticleCategories.DTOs;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Entities;
using Stock_Exchange.Domain.Repositories.Interfaces;
using Stock_Exchange.Domain.Repositories.Interfaces.Base;

namespace Stock_Exchange.Application.Features.ArticleCategories.Commands.AddArticleCategory
{
    public class AddArticleCategoryCommandHandler : IRequestHandler<AddArticleCategoryCommand, Result<ArticleCategoryDto>>
    {
        private readonly IArticleCategoryRepository _categoryRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public AddArticleCategoryCommandHandler(
            IArticleCategoryRepository categoryRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _categoryRepository = categoryRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result<ArticleCategoryDto>> Handle(AddArticleCategoryCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUserService.IsAuthenticated || _currentUserService.UserId == Guid.Empty)
                throw new UnAuthorizedException(LocalizationKeys.ExceptionMessages.Unauthorized);

            var category = new ArticleCategory
            {
                CategoryArName = request.CategoryArName.Trim(),
                CategoryEnName = request.CategoryEnName.Trim()
            };

            await _categoryRepository.AddAsync(category);
            await _unitOfWork.SaveChangesAsync();

            return Result<ArticleCategoryDto>.Success(new ArticleCategoryDto
            {
                Id = category.Id,
                CategoryArName = category.CategoryArName,
                CategoryEnName = category.CategoryEnName,
                ArticlesCount = 0,
                IsActive = category.IsActive,
                CreatedAt = category.CreatedAt
            });
        }
    }
}
