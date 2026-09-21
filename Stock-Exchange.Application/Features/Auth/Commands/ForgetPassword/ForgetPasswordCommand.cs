using MediatR;

namespace Stock_Exchange.Application.Features.Auth.Commands.ForgetPassword
{
    public record ForgetPasswordCommand(string Email) : IRequest<bool>;
}
