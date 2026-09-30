using MediatR;
using Stock_Exchange.Application.Common.Models;

namespace Stock_Exchange.Application.Features.Users.Commands.AdminChangePassword
{
    public record AdminChangePasswordCommand : IRequest<Result<bool>>
    {
        public Guid UserId { get; init; }
        public string NewPassword { get; init; } = null!;
        public string ConfirmNewPassword { get; init; } = null!;

        public AdminChangePasswordCommand()
        {
        }

        public AdminChangePasswordCommand(Guid userId, string newPassword, string confirmNewPassword)
        {
            UserId = userId;
            NewPassword = newPassword;
            ConfirmNewPassword = confirmNewPassword;
        }
    }
}
