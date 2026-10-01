using MediatR;
using Microsoft.EntityFrameworkCore;
using Stock_Exchange.Application.Common.Extensions;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.Countries.DTOs;

namespace Stock_Exchange.Application.Features.Countries.Queries.GetAllCountriesPaginated
{
    public class GetAllCountriesPaginatedQueryHandler : IRequestHandler<GetAllCountriesPaginatedQuery, Result<PagginatedResult<CountryDto>>>
    {
        private readonly IStockExchangeDbContext _dbContext;

        public GetAllCountriesPaginatedQueryHandler(IStockExchangeDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Result<PagginatedResult<CountryDto>>> Handle(GetAllCountriesPaginatedQuery request, CancellationToken cancellationToken)
        {
            var query = from c in _dbContext.Countries.AsNoTracking().Where(c => !c.IsDeleted)
                        select new
                        {
                            Country = c,
                            UsersCount = _dbContext.Users.Count(u => !u.IsDeleted && u.CountryId == c.Id)
                        };

            if (request.IsActive.HasValue)
            {
                query = query.Where(x => x.Country.IsActive == request.IsActive.Value);
            }

            var search = request.Search?.Trim();
            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.ToLower();
                query = query.Where(x =>
                    x.Country.CountryArName.ToLower().Contains(term) ||
                    x.Country.CountryEnName.ToLower().Contains(term) ||
                    x.Country.Code.ToLower().Contains(term));
            }

            var safePageSize = request.PageSize <= 0 ? 10 : request.PageSize;
            var safePageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;

            var projectedQuery = query
                .OrderBy(x => x.Country.CountryEnName)
                .Select(x => new CountryDto
                {
                    Id = x.Country.Id,
                    CountryArName = x.Country.CountryArName,
                    CountryEnName = x.Country.CountryEnName,
                    Code = x.Country.Code,
                    IsActive = x.Country.IsActive,
                    UsersCount = x.UsersCount,
                    CreatedAt = x.Country.CreatedAt
                });

            var pagedList = await projectedQuery.AsPagginatedListAsync(safePageNumber, safePageSize, cancellationToken);

            return Result<PagginatedResult<CountryDto>>.Success(pagedList);
        }
    }
}
