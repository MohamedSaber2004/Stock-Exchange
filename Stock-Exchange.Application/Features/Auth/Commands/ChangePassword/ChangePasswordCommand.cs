using MediatR;

namespace Stock_Exchange.Application.Features.Auth.Commands.ChangePassword
{
    public record ChangePasswordCommand : IRequest<bool>
    {
        public string CurrentPassword { get; init; } = null!;
        public string NewPassword { get; init; } = null!;
        public string ConfirmNewPassword { get; init; } = null!;

        public ChangePasswordCommand()
        {
        }

        public ChangePasswordCommand(string currentPassword, string newPassword, string confirmNewPassword)
        {
            CurrentPassword = currentPassword;
            NewPassword = newPassword;
            ConfirmNewPassword = confirmNewPassword;
        }
    }
}
