using MediatR;

namespace Stock_Exchange.Application.Features.Auth.Commands.DeleteAccount
{
    public record DeleteAccountCommand : IRequest<bool>;
}
