using MediatR;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.HelpCenterCategories.DTOs;

namespace Stock_Exchange.Application.Features.HelpCenterCategories.Queries.GetAllHelpCenterCategoryById
{
    public class GetHelpCenterCategoryByIdQuery : IRequest<Result<HelpCenterCategoryDto>>
    {
        public Guid Id { get; set; }
        public bool? ApplyLanguageFilter { get; set; }

        public GetHelpCenterCategoryByIdQuery()
        {
        }

        public GetHelpCenterCategoryByIdQuery(Guid id, bool? applyLanguageFilter = null)
        {
            Id = id;
            ApplyLanguageFilter = applyLanguageFilter;
        }
    }

    public class GetAllHelpCenterCategoryQueryById : GetHelpCenterCategoryByIdQuery
    {
        public GetAllHelpCenterCategoryQueryById()
        {
        }

        public GetAllHelpCenterCategoryQueryById(Guid id, bool? applyLanguageFilter = null) : base(id, applyLanguageFilter)
        {
        }
    }
}
