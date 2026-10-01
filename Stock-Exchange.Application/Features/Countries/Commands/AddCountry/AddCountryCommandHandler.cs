using MediatR;
using Microsoft.EntityFrameworkCore;
using Stock_Exchange.Application.Common.Exceptions;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.Countries.DTOs;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Entities;
using Stock_Exchange.Domain.Repositories.Interfaces;
using Stock_Exchange.Domain.Repositories.Interfaces.Base;

namespace Stock_Exchange.Application.Features.Countries.Commands.AddCountry
{
    public class AddCountryCommandHandler : IRequestHandler<AddCountryCommand, Result<CountryDto>>
    {
        private readonly ICountryRepository _countryRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public AddCountryCommandHandler(
            ICountryRepository countryRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _countryRepository = countryRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Result<CountryDto>> Handle(AddCountryCommand request, CancellationToken cancellationToken)
        {
            var cleanArName = request.CountryArName.Trim();
            var cleanEnName = request.CountryEnName.Trim();
            var cleanCode = request.Code.Trim();
            if (!cleanCode.StartsWith("+"))
            {
                cleanCode = "+" + cleanCode;
            }

            var exists = await _countryRepository.GetAllAsync(c => !c.IsDeleted &&
                (c.Code == cleanCode ||
                 c.CountryArName.ToLower() == cleanArName.ToLower() ||
                 c.CountryEnName.ToLower() == cleanEnName.ToLower()))
                .AnyAsync(cancellationToken);

            if (exists)
            {
                throw new ConflictException(LocalizationKeys.CountryMessages.CountryAlreadyExists);
            }

            var country = new Country
            {
                CountryArName = cleanArName,
                CountryEnName = cleanEnName,
                Code = cleanCode
            };

            var creator = _currentUserService.UserId != Guid.Empty
                ? _currentUserService.UserId.ToString()
                : "Admin";

            country.MarkAsCreated(creator);
            if (!request.IsActive)
            {
                country.Deactive();
            }

            await _countryRepository.AddAsync(country);
            await _unitOfWork.SaveChangesAsync();

            var dto = new CountryDto
            {
                Id = country.Id,
                CountryArName = country.CountryArName,
                CountryEnName = country.CountryEnName,
                Code = country.Code,
                IsActive = country.IsActive,
                UsersCount = 0,
                CreatedAt = country.CreatedAt
            };

            return Result<CountryDto>.Success(dto);
        }
    }
}
