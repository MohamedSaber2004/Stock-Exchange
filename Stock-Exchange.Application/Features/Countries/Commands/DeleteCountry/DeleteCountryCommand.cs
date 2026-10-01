using MediatR;
using Stock_Exchange.Application.Common.Models;

namespace Stock_Exchange.Application.Features.Countries.Commands.DeleteCountry
{
    public record DeleteCountryCommand(Guid Id) : IRequest<Result<bool>>;
}
