using MediatR;
using Microsoft.EntityFrameworkCore;
using Stock_Exchange.Application.Common.Exceptions;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.Countries.DTOs;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Repositories.Interfaces;

namespace Stock_Exchange.Application.Features.Countries.Queries.GetCountryById
{
    public class GetCountryByIdQueryHandler : IRequestHandler<GetCountryByIdQuery, Result<CountryDto>>
    {
        private readonly ICountryRepository _countryRepository;
        private readonly IStockExchangeDbContext _dbContext;

        public GetCountryByIdQueryHandler(
            ICountryRepository countryRepository,
            IStockExchangeDbContext dbContext)
        {
            _countryRepository = countryRepository;
            _dbContext = dbContext;
        }

        public async Task<Result<CountryDto>> Handle(GetCountryByIdQuery request, CancellationToken cancellationToken)
        {
            var country = await _countryRepository.GetByIdAsync(request.Id);
            if (country == null || country.IsDeleted)
            {
                throw new NotFoundException(LocalizationKeys.CountryMessages.CountryNotFound);
            }

            var usersCount = await _dbContext.Users
                .AsNoTracking()
                .CountAsync(u => !u.IsDeleted && u.CountryId == country.Id, cancellationToken);

            var dto = new CountryDto
            {
                Id = country.Id,
                CountryArName = country.CountryArName,
                CountryEnName = country.CountryEnName,
                Code = country.Code,
                IsActive = country.IsActive,
                UsersCount = usersCount,
                CreatedAt = country.CreatedAt
            };

            return Result<CountryDto>.Success(dto);
        }
    }
}
