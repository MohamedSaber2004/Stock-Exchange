using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Entities;

namespace Stock_Exchange.Application.Features.Auth.Commands.Register
{
    public class SignupCommandValidator : AbstractValidator<SignupCommand>
    {
        public SignupCommandValidator(UserManager<ApplicationUser> userManager)
        {
            RuleFor(x => x.FullName)
                .NotEmpty()
                .WithMessage(LocalizationKeys.AuthMessages.FullNameRequired);

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage(LocalizationKeys.AuthMessages.EmailRequired)
                .EmailAddress()
                .WithMessage(LocalizationKeys.AuthMessages.InvalidEmail);

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage(LocalizationKeys.AuthMessages.PasswordRequired);

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty()
                .WithMessage(LocalizationKeys.AuthMessages.ConfirmPasswordRequired)
                .Equal(x => x.Password)
                .WithMessage(LocalizationKeys.AuthMessages.PasswordsDoNotMatch);

            RuleFor(x => x.PhoneNumber)
                .NotEmpty()
                .WithMessage(LocalizationKeys.AuthMessages.PhoneNumberRequired)
                .Matches(@"^(?:\+20|0020)?0?1[0125][0-9]{8}$")
                .WithMessage(LocalizationKeys.AuthMessages.InvalidPhoneNumber)
                .MustAsync(async (command, phoneNumber, cancellationToken) =>
                {
                    if (string.IsNullOrWhiteSpace(phoneNumber))
                        return true;

                    var rawPhone = phoneNumber.Trim();
                    var normalizedPhone = NormalizePhoneNumber(rawPhone);
                    var internationalPhone = "+20" + (normalizedPhone.StartsWith("0") ? normalizedPhone[1..] : normalizedPhone);

                    var isTakenByOtherClient = await userManager.Users
                        .AnyAsync(u => !u.IsDeleted &&
                                       u.Email != command.Email &&
                                       (u.PhoneNumber == rawPhone ||
                                        u.PhoneNumber == normalizedPhone ||
                                        u.PhoneNumber == internationalPhone),
                                  cancellationToken);

                    return !isTakenByOtherClient;
                })
                .WithMessage(LocalizationKeys.AuthMessages.PhoneNumberAlreadyExists);
        }

        private static string NormalizePhoneNumber(string phoneNumber)
        {
            var cleaned = phoneNumber.Trim().Replace(" ", "").Replace("-", "");
            if (cleaned.StartsWith("+20"))
                cleaned = cleaned[3..];
            else if (cleaned.StartsWith("0020"))
                cleaned = cleaned[4..];

            if (!cleaned.StartsWith("0") && cleaned.StartsWith("1"))
                cleaned = "0" + cleaned;

            return cleaned;
        }
    }
}
