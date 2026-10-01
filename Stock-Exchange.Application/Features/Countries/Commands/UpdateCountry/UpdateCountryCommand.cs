using MediatR;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Application.Features.Countries.DTOs;

namespace Stock_Exchange.Application.Features.Countries.Commands.UpdateCountry
{
    public record UpdateCountryCommand : IRequest<Result<CountryDto>>
    {
        public Guid Id { get; init; }
        public string CountryArName { get; init; } = string.Empty;
        public string CountryEnName { get; init; } = string.Empty;
        public string Code { get; init; } = string.Empty;
        public bool IsActive { get; init; } = true;
    }
}
