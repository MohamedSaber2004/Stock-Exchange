using MediatR;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.HelpCenterCategories.DTOs;

namespace Stock_Exchange.Application.Features.HelpCenterCategories.Queries.GetAllHelpCenterCategories
{
    public class GetAllHelpCenterCategoriesQuery : IRequest<Result<List<HelpCenterCategoryDto>>>
    {
        public string? Search { get; set; }

        public GetAllHelpCenterCategoriesQuery()
        {
        }

        public GetAllHelpCenterCategoriesQuery(string? search)
        {
            Search = search;
        }
    }
}
