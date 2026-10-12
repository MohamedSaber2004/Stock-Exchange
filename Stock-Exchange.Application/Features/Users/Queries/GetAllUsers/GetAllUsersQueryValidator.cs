using FluentValidation;
using Stock_Exchange.Application.Localization;

namespace Stock_Exchange.Application.Features.Users.Queries.GetAllUsers
{
    public class GetAllUsersQueryValidator : AbstractValidator<GetAllUsersQuery>
    {
        public GetAllUsersQueryValidator()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThanOrEqualTo(1)
                .WithMessage(LocalizationKeys.CommonMessages.PageNumberInvalid);

            RuleFor(x => x.PageSize)
                .GreaterThan(0)
                .WithMessage(LocalizationKeys.CommonMessages.PageSizeInvalid)
                .LessThanOrEqualTo(100)
                .WithMessage(LocalizationKeys.CommonMessages.PageSizeTooLarge);

            RuleFor(x => x.Search)
                .MaximumLength(100)
                .WithMessage(LocalizationKeys.CommonMessages.SearchTooLong)
                .When(x => !string.IsNullOrEmpty(x.Search));
        }
    }
}
