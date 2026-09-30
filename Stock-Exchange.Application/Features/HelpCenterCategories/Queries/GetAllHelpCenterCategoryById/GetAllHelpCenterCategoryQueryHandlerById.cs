using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.HelpCenterCategories.DTOs;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Repositories.Interfaces;

namespace Stock_Exchange.Application.Features.HelpCenterCategories.Queries.GetAllHelpCenterCategoryById
{
    internal static class HelpCenterCategoryByIdQueryExecutor
    {
        public static async Task<Result<HelpCenterCategoryDto>> ExecuteAsync(
            Guid id,
            IHelpCenterCategoryRepository categoryRepository,
            ICurrentLanguageService currentLanguageService,
            CancellationToken cancellationToken)
        {
            var category = await categoryRepository
                .GetAllAsync(c => c.Id == id && c.IsActive)
                .AsNoTracking()
                .Select(c => new HelpCenterCategoryDto
                {
                    Id = c.Id,
                    TitleEn = c.TitleEn,
                    TitleAr = c.TitleAr
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (category is null)
                return Result<HelpCenterCategoryDto>.Failure(LocalizationKeys.HelpCenterMessages.CategoryNotFound, StatusCodes.Status404NotFound);

            category.ApplyLanguageFilter(currentLanguageService.Language);

            return Result<HelpCenterCategoryDto>.Success(category);
        }
    }

    public class GetHelpCenterCategoryByIdQueryHandler
        : IRequestHandler<GetHelpCenterCategoryByIdQuery, Result<HelpCenterCategoryDto>>
    {
        private readonly IHelpCenterCategoryRepository _categoryRepository;
        private readonly ICurrentLanguageService _currentLanguageService;

        public GetHelpCenterCategoryByIdQueryHandler(
            IHelpCenterCategoryRepository categoryRepository,
            ICurrentLanguageService currentLanguageService)
        {
            _categoryRepository = categoryRepository;
            _currentLanguageService = currentLanguageService;
        }

        public Task<Result<HelpCenterCategoryDto>> Handle(GetHelpCenterCategoryByIdQuery request, CancellationToken cancellationToken)
        {
            return HelpCenterCategoryByIdQueryExecutor.ExecuteAsync(
                request.Id,
                _categoryRepository,
                _currentLanguageService,
                cancellationToken);
        }
    }

    public class GetAllHelpCenterCategoryQueryHandlerById
        : IRequestHandler<GetAllHelpCenterCategoryQueryById, Result<HelpCenterCategoryDto>>
    {
        private readonly IHelpCenterCategoryRepository _categoryRepository;
        private readonly ICurrentLanguageService _currentLanguageService;

        public GetAllHelpCenterCategoryQueryHandlerById(
            IHelpCenterCategoryRepository categoryRepository,
            ICurrentLanguageService currentLanguageService)
        {
            _categoryRepository = categoryRepository;
            _currentLanguageService = currentLanguageService;
        }

        public Task<Result<HelpCenterCategoryDto>> Handle(GetAllHelpCenterCategoryQueryById request, CancellationToken cancellationToken)
        {
            return HelpCenterCategoryByIdQueryExecutor.ExecuteAsync(
                request.Id,
                _categoryRepository,
                _currentLanguageService,
                cancellationToken);
        }
    }
}
