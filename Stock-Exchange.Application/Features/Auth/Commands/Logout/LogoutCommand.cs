using MediatR;

namespace Stock_Exchange.Application.Features.Auth.Commands.Logout
{
    public record LogoutCommand : IRequest<bool>
    {
        public string? RefreshToken { get; set; }
    }
}
