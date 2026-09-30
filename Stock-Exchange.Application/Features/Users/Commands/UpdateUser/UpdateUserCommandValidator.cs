using FluentValidation;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Repositories.Interfaces;

namespace Stock_Exchange.Application.Features.Users.Commands.UpdateUser
{
    public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
    {
        public UpdateUserCommandValidator(ICountryRepository countryRepository)
        {
            RuleFor(x => x.Id)
                .NotEmpty();

            RuleFor(x => x.FullName)
                .NotEmpty()
                .WithMessage(LocalizationKeys.AuthMessages.FullNameRequired)
                .MaximumLength(150);

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage(LocalizationKeys.AuthMessages.EmailRequired)
                .EmailAddress()
                .WithMessage(LocalizationKeys.AuthMessages.InvalidEmail);

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
