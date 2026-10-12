using FluentValidation;
using Stock_Exchange.Application.Localization;

namespace Stock_Exchange.Application.Features.ActivityLogs.Queries.GetAllActivityLogs
{
    public class GetAllActivityLogsQueryValidator : AbstractValidator<GetAllActivityLogsQuery>
    {
        public GetAllActivityLogsQueryValidator()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThanOrEqualTo(1)
                .WithMessage(LocalizationKeys.ActivityLogMessages.PageNumberInvalid);

            RuleFor(x => x.PageSize)
                .GreaterThan(0)
                .WithMessage(LocalizationKeys.ActivityLogMessages.PageSizeInvalid)
                .LessThanOrEqualTo(100)
                .WithMessage(LocalizationKeys.ActivityLogMessages.PageSizeTooLarge);

            RuleFor(x => x.Search)
                .MaximumLength(100)
                .WithMessage(LocalizationKeys.ActivityLogMessages.SearchTooLong)
                .When(x => !string.IsNullOrEmpty(x.Search));
        }
    }
}
