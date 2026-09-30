using FluentValidation;

namespace Stock_Exchange.Application.Features.ActivityLogs.Queries.GetActivityLogById
{
    public class GetActivityLogByIdQueryValidator : AbstractValidator<GetActivityLogByIdQuery>
    {
        public GetActivityLogByIdQueryValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty();
        }
    }
}
