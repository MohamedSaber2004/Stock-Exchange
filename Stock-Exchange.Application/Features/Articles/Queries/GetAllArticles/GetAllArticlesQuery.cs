using MediatR;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.Articles.DTOs;

namespace Stock_Exchange.Application.Features.Articles.Queries.GetAllArticles
{
    public class GetAllArticlesQuery : IRequest<Result<PagginatedResult<ArticleDto>>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Search { get; set; }
        public Guid? CategoryId { get; set; }
        public bool? IsActive { get; set; }
        public bool ApplyLanguageFilter { get; set; } = true;

        public GetAllArticlesQuery()
        {
        }

        public GetAllArticlesQuery(int pageNumber, int pageSize, string? search = null, Guid? categoryId = null, bool? isActive = null, bool applyLanguageFilter = true)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            Search = search;
            CategoryId = categoryId;
            IsActive = isActive;
            ApplyLanguageFilter = applyLanguageFilter;
        }
    }
}
