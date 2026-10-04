using MediatR;
using Stock_Exchange.Application.Common.Models;

namespace Stock_Exchange.Application.Features.Experts.Commands.DeleteExpert
{
    public record DeleteExpertCommand(Guid Id) : IRequest<Result<bool>>;
}
