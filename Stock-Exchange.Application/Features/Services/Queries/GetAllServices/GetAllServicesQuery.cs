using MediatR;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.Services.DTOs;

namespace Stock_Exchange.Application.Features.Services.Queries.GetAllServices
{
    public class GetAllServicesQuery : IRequest<Result<PagginatedResult<ServiceDto>>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
        public bool ApplyLanguageFilter { get; set; } = true;
    }
}
