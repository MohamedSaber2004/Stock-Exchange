using MediatR;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.Countries.DTOs;

namespace Stock_Exchange.Application.Features.Countries.Queries.GetAllCountriesPaginated
{
    public record GetAllCountriesPaginatedQuery : IRequest<Result<PagginatedResult<CountryDto>>>
    {
        public string? Search { get; init; }
        public bool? IsActive { get; init; }
        public int PageNumber { get; init; } = 1;
        public int PageSize { get; init; } = 10;
    }
}
