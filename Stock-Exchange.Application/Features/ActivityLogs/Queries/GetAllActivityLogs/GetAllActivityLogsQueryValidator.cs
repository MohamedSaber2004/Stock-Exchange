using FluentValidation;

namespace Stock_Exchange.Application.Features.ActivityLogs.Queries.GetAllActivityLogs
{
    public class GetAllActivityLogsQueryValidator : AbstractValidator<GetAllActivityLogsQuery>
    {
        public GetAllActivityLogsQueryValidator()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThanOrEqualTo(1);

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100);

            RuleFor(x => x.Search)
                .MaximumLength(100)
                .When(x => !string.IsNullOrEmpty(x.Search));
        }
    }
}
