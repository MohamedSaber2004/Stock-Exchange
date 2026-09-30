using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Entities;
using Stock_Exchange.Domain.Repositories.Interfaces;

namespace Stock_Exchange.Application.Features.Users.Commands.AddUser
{
    public class AddUserCommandValidator : AbstractValidator<AddUserCommand>
    {
        public AddUserCommandValidator(
            UserManager<ApplicationUser> userManager,
            ICountryRepository countryRepository)
        {
            RuleFor(x => x.FullName)
                .NotEmpty()
                .WithMessage(LocalizationKeys.AuthMessages.FullNameRequired)
                .MaximumLength(150);

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage(LocalizationKeys.AuthMessages.EmailRequired)
                .EmailAddress()
                .WithMessage(LocalizationKeys.AuthMessages.InvalidEmail)
                .MustAsync(async (email, cancellationToken) =>
                {
                    if (string.IsNullOrWhiteSpace(email))
                        return true;

                    var normalized = email.Trim().ToUpperInvariant();
                    var exists = await userManager.Users
                        .IgnoreQueryFilters()
                        .AsNoTracking()
                        .AnyAsync(u => u.NormalizedEmail == normalized, cancellationToken);

                    return !exists;
                })
                .WithMessage(LocalizationKeys.AuthMessages.EmailAlreadyExists);

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage(LocalizationKeys.AuthMessages.PasswordRequired)
                .MinimumLength(6);

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty()
                .WithMessage(LocalizationKeys.AuthMessages.ConfirmPasswordRequired)
                .Equal(x => x.Password)
                .WithMessage(LocalizationKeys.AuthMessages.PasswordsDoNotMatch);

            RuleFor(x => x.PhoneNumber)
                .Matches(@"^[0-9+\-\s()]{6,20}$")
                .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber))
                .WithMessage(LocalizationKeys.AuthMessages.InvalidPhoneNumber);

            RuleFor(x => x.CountryId)
                .MustAsync(async (countryId, cancellationToken) =>
                {
                    if (!countryId.HasValue || countryId.Value == Guid.Empty)
                        return true;

                    return await countryRepository.ExistsAsync(
                        c => c.Id == countryId.Value && !c.IsDeleted && c.IsActive,
                        cancellationToken);
                })
                .When(x => x.CountryId.HasValue)
                .WithMessage(LocalizationKeys.CountryMessages.CountryNotFound);
        }
    }
}
