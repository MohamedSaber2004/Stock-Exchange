using MediatR;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.HelpCenterCategories.DTOs;

namespace Stock_Exchange.Application.Features.HelpCenterCategories.Queries.GetAllHelpCenterCategoryById
{
    public class GetHelpCenterCategoryByIdQuery : IRequest<Result<HelpCenterCategoryDto>>
    {
        public Guid Id { get; set; }

        public GetHelpCenterCategoryByIdQuery()
        {
        }

        public GetHelpCenterCategoryByIdQuery(Guid id)
        {
            Id = id;
        }
    }

    public class GetAllHelpCenterCategoryQueryById : GetHelpCenterCategoryByIdQuery
    {
        public GetAllHelpCenterCategoryQueryById()
        {
        }

        public GetAllHelpCenterCategoryQueryById(Guid id) : base(id)
        {
        }
    }
}
