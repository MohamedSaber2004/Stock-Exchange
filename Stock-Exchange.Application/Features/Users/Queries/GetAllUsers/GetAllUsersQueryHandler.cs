using MediatR;
using Microsoft.EntityFrameworkCore;
using Stock_Exchange.Application.Common.Extensions;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.Users.DTOs;
using Stock_Exchange.Domain.Enums;

namespace Stock_Exchange.Application.Features.Users.Queries.GetAllUsers
{
    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, Result<PagginatedResult<UserDto>>>
    {
        private readonly IStockExchangeDbContext _dbContext;
        private readonly ICurrentLanguageService _currentLanguageService;

        public GetAllUsersQueryHandler(
            IStockExchangeDbContext dbContext,
            ICurrentLanguageService currentLanguageService)
        {
            _dbContext = dbContext;
            _currentLanguageService = currentLanguageService;
        }

        public async Task<Result<PagginatedResult<UserDto>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            var query = from u in _dbContext.Users.AsNoTracking().Where(u => !u.IsDeleted)
                        join c in _dbContext.Countries.AsNoTracking() on u.CountryId equals c.Id into countries
                        from c in countries.DefaultIfEmpty()
                        select new
                        {
                            User = u,
                            Country = c,
                            RoleName = (from ur in _dbContext.UserRoles.AsNoTracking()
                                        join r in _dbContext.Roles.AsNoTracking() on ur.RoleId equals r.Id
                                        where ur.UserId == u.Id
                                        select r.Name).FirstOrDefault()
                        };

            if (request.IsActive.HasValue)
            {
                query = query.Where(x => x.User.IsActive == request.IsActive.Value);
            }

            if (request.UserType.HasValue)
            {
                var targetRole = request.UserType.Value.ToString();
                query = query.Where(x => x.RoleName == targetRole);
            }

            var search = request.Search?.Trim();
            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.ToLower();
                query = query.Where(x =>
                    x.User.FullName.ToLower().Contains(term) ||
                    (x.User.Email != null && x.User.Email.ToLower().Contains(term)) ||
                    (x.User.PhoneNumber != null && x.User.PhoneNumber.ToLower().Contains(term)) ||
                    (x.Country != null && (
                        x.Country.CountryArName.ToLower().Contains(term) ||
                        x.Country.CountryEnName.ToLower().Contains(term) ||
                        x.Country.Code.ToLower().Contains(term)
                    )));
            }

            var safePageSize = request.PageSize <= 0 ? 10 : request.PageSize;
            var safePageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;

            var projectedQuery = query
                .OrderByDescending(x => x.User.CreatedAt)
                .Select(x => new UserDto
                {
                    Id = x.User.Id,
                    FullName = x.User.FullName,
                    Email = x.User.Email ?? string.Empty,
                    PhoneNumber = x.User.PhoneNumber,
                    ProfilePictureUrl = x.User.ProfilePictureUrl,
                    UserType = x.RoleName == "Admin" ? UserType.Admin : UserType.Customer,
                    IsActive = x.User.IsActive,
                    CreatedAt = x.User.CreatedAt,
                    CountryId = x.User.CountryId,
                    CountryArName = x.Country != null ? x.Country.CountryArName : null,
                    CountryEnName = x.Country != null ? x.Country.CountryEnName : null,
                    CountryCode = x.Country != null ? x.Country.Code : null
                });

            var pagedUsers = await projectedQuery.AsPagginatedListAsync(safePageNumber, safePageSize, cancellationToken);

            var language = _currentLanguageService.Language;
            foreach (var item in pagedUsers.Items)
            {
                item.ApplyLanguageFilter(language);
            }

            return Result<PagginatedResult<UserDto>>.Success(pagedUsers);
        }
    }
}
