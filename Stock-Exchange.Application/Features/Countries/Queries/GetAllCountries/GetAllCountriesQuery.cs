using MediatR;
using Stock_Exchange.Application.Features.Countries.DTOs;

namespace Stock_Exchange.Application.Features.Countries.Queries.GetAllCountries
{
    public class GetAllCountriesQuery : IRequest<List<CountryDto>>
    {
    }
}
