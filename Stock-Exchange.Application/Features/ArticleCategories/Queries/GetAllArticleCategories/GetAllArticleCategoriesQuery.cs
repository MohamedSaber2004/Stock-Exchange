using MediatR;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.ArticleCategories.DTOs;

namespace Stock_Exchange.Application.Features.ArticleCategories.Queries.GetAllArticleCategories
{
    public class GetAllArticleCategoriesQuery : IRequest<Result<List<ArticleCategoryDto>>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
        public bool? ApplyLanguageFilter { get; set; }

        public GetAllArticleCategoriesQuery()
        {
        }

        public GetAllArticleCategoriesQuery(string? search, bool? isActive = null, bool? applyLanguageFilter = null)
        {
            Search = search;
            IsActive = isActive;
            ApplyLanguageFilter = applyLanguageFilter;
        }
    }
}
