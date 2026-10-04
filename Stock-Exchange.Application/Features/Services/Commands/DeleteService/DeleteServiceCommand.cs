using MediatR;
using Stock_Exchange.Application.Common.Models;

namespace Stock_Exchange.Application.Features.Services.Commands.DeleteService
{
    public record DeleteServiceCommand(Guid Id) : IRequest<Result<bool>>;
}
