using MediatR;
using Microsoft.EntityFrameworkCore;
using Stock_Exchange.Application.Common.Exceptions;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Repositories.Interfaces;
using Stock_Exchange.Domain.Repositories.Interfaces.Base;

namespace Stock_Exchange.Application.Features.Countries.Commands.DeleteCountry
{
    public class DeleteCountryCommandHandler : IRequestHandler<DeleteCountryCommand, Result<bool>>
    {
        private readonly ICountryRepository _countryRepository;
        private readonly IStockExchangeDbContext _dbContext;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public DeleteCountryCommandHandler(
            ICountryRepository countryRepository,
            IStockExchangeDbContext dbContext,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _countryRepository = countryRepository;
            _dbContext = dbContext;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result<bool>> Handle(DeleteCountryCommand request, CancellationToken cancellationToken)
        {
            var country = await _countryRepository.GetByIdAsync(request.Id);
            if (country == null || country.IsDeleted)
            {
                throw new NotFoundException(LocalizationKeys.CountryMessages.CountryNotFound);
            }

            var hasUsers = await _dbContext.Users
                .AsNoTracking()
                .AnyAsync(u => !u.IsDeleted && u.CountryId == country.Id, cancellationToken);

            if (hasUsers)
            {
                throw new BadRequestException(LocalizationKeys.CountryMessages.CountryCannotBeDeleted);
            }

            var deleter = _currentUserService.UserId != Guid.Empty
                ? _currentUserService.UserId.ToString()
                : "Admin";

            country.MarkAsDeleted(deleter);
            _countryRepository.Update(country);
            await _unitOfWork.SaveChangesAsync();

            return Result<bool>.Success(true);
        }
    }
}
