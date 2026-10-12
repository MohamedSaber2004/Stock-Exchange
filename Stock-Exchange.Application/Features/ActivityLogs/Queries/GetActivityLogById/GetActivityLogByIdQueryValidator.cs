using FluentValidation;
using Stock_Exchange.Application.Localization;

namespace Stock_Exchange.Application.Features.ActivityLogs.Queries.GetActivityLogById
{
    public class GetActivityLogByIdQueryValidator : AbstractValidator<GetActivityLogByIdQuery>
    {
        public GetActivityLogByIdQueryValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage(LocalizationKeys.ActivityLogMessages.IdRequired);
        }
    }
}
