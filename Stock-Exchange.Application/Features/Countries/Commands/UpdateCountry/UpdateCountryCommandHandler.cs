using MediatR;
using Microsoft.EntityFrameworkCore;
using Stock_Exchange.Application.Common.Exceptions;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.Countries.DTOs;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Repositories.Interfaces;
using Stock_Exchange.Domain.Repositories.Interfaces.Base;

namespace Stock_Exchange.Application.Features.Countries.Commands.UpdateCountry
{
    public class UpdateCountryCommandHandler : IRequestHandler<UpdateCountryCommand, Result<CountryDto>>
    {
        private readonly ICountryRepository _countryRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IStockExchangeDbContext _dbContext;

        public UpdateCountryCommandHandler(
            ICountryRepository countryRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            IStockExchangeDbContext dbContext)
        {
            _countryRepository = countryRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _dbContext = dbContext;
        }

        public async Task<Result<CountryDto>> Handle(UpdateCountryCommand request, CancellationToken cancellationToken)
        {
            var country = await _countryRepository.GetByIdAsync(request.Id);
            if (country == null || country.IsDeleted)
            {
                throw new NotFoundException(LocalizationKeys.CountryMessages.CountryNotFound);
            }

            var cleanArName = request.CountryArName.Trim();
            var cleanEnName = request.CountryEnName.Trim();
            var cleanCode = request.Code.Trim();
            if (!cleanCode.StartsWith("+"))
            {
                cleanCode = "+" + cleanCode;
            }

            var exists = await _countryRepository.GetAllAsync(c => !c.IsDeleted && c.Id != country.Id &&
                (c.Code == cleanCode ||
                 c.CountryArName.ToLower() == cleanArName.ToLower() ||
                 c.CountryEnName.ToLower() == cleanEnName.ToLower()))
                .AnyAsync(cancellationToken);

            if (exists)
            {
                throw new ConflictException(LocalizationKeys.CountryMessages.CountryAlreadyExists);
            }

            country.CountryArName = cleanArName;
            country.CountryEnName = cleanEnName;
            country.Code = cleanCode;

            var updater = _currentUserService.UserId != Guid.Empty
                ? _currentUserService.UserId.ToString()
                : "Admin";

            country.SetActiveState(request.IsActive, updater);

            _countryRepository.Update(country);
            await _unitOfWork.SaveChangesAsync();

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
