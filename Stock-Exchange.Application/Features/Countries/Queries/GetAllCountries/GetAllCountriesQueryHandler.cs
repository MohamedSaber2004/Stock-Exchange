using MediatR;
using Microsoft.EntityFrameworkCore;
using Stock_Exchange.Application.Features.Countries.DTOs;
using Stock_Exchange.Domain.Repositories.Interfaces;

namespace Stock_Exchange.Application.Features.Countries.Queries.GetAllCountries
{
    public class GetAllCountriesQueryHandler : IRequestHandler<GetAllCountriesQuery, List<CountryDto>>
    {
        private readonly ICountryRepository _countryRepository;

        public GetAllCountriesQueryHandler(ICountryRepository countryRepository)
        {
            _countryRepository = countryRepository;
        }

        public async Task<List<CountryDto>> Handle(GetAllCountriesQuery request, CancellationToken cancellationToken)
        {
            return await _countryRepository.GetAllAsync(c => !c.IsDeleted && c.IsActive)
                .AsNoTracking()
                .OrderBy(c => c.CountryEnName)
                .Select(c => new CountryDto
                {
                    Id = c.Id,
                    CountryArName = c.CountryArName,
                    CountryEnName = c.CountryEnName,
                    Code = c.Code
                })
                .ToListAsync(cancellationToken);
        }
    }
}
