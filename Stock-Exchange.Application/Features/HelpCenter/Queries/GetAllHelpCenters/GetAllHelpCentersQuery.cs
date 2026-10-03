using MediatR;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.HelpCenter.DTOs;

namespace Stock_Exchange.Application.Features.HelpCenter.Queries.GetAllHelpCenters
{
    public class GetAllHelpCentersQuery : IRequest<Result<List<HelpCenterDto>>>
    {
        public Guid? CategoryId { get; set; }
        public string? Search { get; set; }
        public bool? ApplyLanguageFilter { get; set; }

        public GetAllHelpCentersQuery()
        {
        }

        public GetAllHelpCentersQuery(Guid? categoryId = null, string? search = null, bool? applyLanguageFilter = null)
        {
            CategoryId = categoryId;
            Search = search;
            ApplyLanguageFilter = applyLanguageFilter;
        }
    }
}
