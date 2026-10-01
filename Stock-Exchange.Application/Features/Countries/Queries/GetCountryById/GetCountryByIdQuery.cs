using MediatR;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.Countries.DTOs;

namespace Stock_Exchange.Application.Features.Countries.Queries.GetCountryById
{
    public record GetCountryByIdQuery(Guid Id) : IRequest<Result<CountryDto>>;
}
