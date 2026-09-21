using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Entities;

namespace Stock_Exchange.Application.Features.Auth.Commands.Login
{
    public class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public LoginCommandValidator(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;


            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage(LocalizationKeys.AuthMessages.EmailRequired)
                .EmailAddress()
                .WithMessage(LocalizationKeys.AuthMessages.InvalidEmail)
                .MustAsync(EmailExists)
                .WithMessage(LocalizationKeys.ExceptionMessages.NotFound);

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage(LocalizationKeys.AuthMessages.PasswordRequired);
        }

        private async Task<bool> EmailExists(string email, CancellationToken cancellationToken)
        {
            return await _userManager.Users.AnyAsync(u => u.Email == email);
        }
    }
}
