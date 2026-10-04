using MediatR;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.Experts.DTOs;

namespace Stock_Exchange.Application.Features.Experts.Queries.GetAllExperts
{
    public class GetAllExpertsQuery : IRequest<Result<PagginatedResult<ExpertDto>>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsFeaturedOnHome { get; set; }
        public bool ApplyLanguageFilter { get; set; } = true;
    }
}
