using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Entities;

namespace Stock_Exchange.Application.Features.Auth.Commands.ForgetPassword
{
    public class ForgetPasswordCommandValidator : AbstractValidator<ForgetPasswordCommand>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ForgetPasswordCommandValidator(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage(LocalizationKeys.AuthMessages.EmailRequired)
                .EmailAddress()
                .WithMessage(LocalizationKeys.AuthMessages.InvalidEmail)
                .MustAsync(EmailExists)
                .WithMessage(LocalizationKeys.ExceptionMessages.NotFound);
        }

        private async Task<bool> EmailExists(string email, CancellationToken cancellationToken)
        {
            return await _userManager.Users.AnyAsync(u => u.Email == email && !u.IsDeleted, cancellationToken);
        }
    }
}
