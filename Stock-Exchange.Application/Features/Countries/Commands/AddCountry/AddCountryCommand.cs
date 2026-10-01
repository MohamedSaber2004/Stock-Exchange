using MediatR;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.Countries.DTOs;

namespace Stock_Exchange.Application.Features.Countries.Commands.AddCountry
{
    public record AddCountryCommand : IRequest<Result<CountryDto>>
    {
        public string CountryArName { get; init; } = string.Empty;
        public string CountryEnName { get; init; } = string.Empty;
        public string Code { get; init; } = string.Empty;
        public bool IsActive { get; init; } = true;
    }
}
