using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Entities;
using Stock_Exchange.Domain.Repositories.Interfaces;

namespace Stock_Exchange.Application.Features.Auth.Commands.Register
{
    public class SignupCommandValidator : AbstractValidator<SignupCommand>
    {
        public SignupCommandValidator(
            UserManager<ApplicationUser> userManager,
            ICountryRepository countryRepository)
        {
            RuleFor(x => x.FullName)
                .NotEmpty()
                .WithMessage(LocalizationKeys.AuthMessages.FullNameRequired)
                .MinimumLength(2)
                .WithMessage(LocalizationKeys.AuthMessages.FullNameRequired)
                .MaximumLength(150)
                .WithMessage(LocalizationKeys.AuthMessages.FullNameRequired);

            RuleFor(x => x.CountryId)
                .NotEmpty()
                .WithMessage(LocalizationKeys.CountryMessages.CountryNotFound)
                .MustAsync(async (countryId, cancellationToken) =>
                {
                    if (countryId == Guid.Empty)
                        return false;

                    return await countryRepository.ExistsAsync(
                        c => c.Id == countryId && !c.IsDeleted && c.IsActive,
                        cancellationToken);
                })
                .WithMessage(LocalizationKeys.CountryMessages.CountryNotFound);

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage(LocalizationKeys.AuthMessages.EmailRequired)
                .EmailAddress()
                .WithMessage(LocalizationKeys.AuthMessages.InvalidEmail)
                .MustAsync(async (email, cancellationToken) =>
                {
                    if (string.IsNullOrWhiteSpace(email))
                        return true;

                    var normalizedEmail = email.Trim().ToUpperInvariant();
                    var emailExists = await userManager.Users
                        .IgnoreQueryFilters()
                        .AsNoTracking()
                        .AnyAsync(u => u.NormalizedEmail == normalizedEmail, cancellationToken);

                    return !emailExists;
                })
                .WithMessage(LocalizationKeys.AuthMessages.EmailAlreadyExists);

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
                .Matches(@"^[0-9+\-\s()]{6,20}$")
                .WithMessage(LocalizationKeys.AuthMessages.InvalidPhoneNumber)
                .MustAsync(async (phoneNumber, cancellationToken) =>
                {
                    if (string.IsNullOrWhiteSpace(phoneNumber))
                        return true;

                    var rawPhone = phoneNumber.Trim();
                    var cleanPhone = CleanPhoneNumber(rawPhone);

                    var phoneExists = await userManager.Users
                        .IgnoreQueryFilters()
                        .AsNoTracking()
                        .AnyAsync(u => u.PhoneNumber == rawPhone || u.PhoneNumber == cleanPhone,
                                  cancellationToken);

                    return !phoneExists;
                })
                .WithMessage(LocalizationKeys.AuthMessages.PhoneNumberAlreadyExists);
        }

        private static string CleanPhoneNumber(string phoneNumber)
        {
            return phoneNumber.Trim().Replace(" ", "").Replace("-", "");
        }
    }
}
