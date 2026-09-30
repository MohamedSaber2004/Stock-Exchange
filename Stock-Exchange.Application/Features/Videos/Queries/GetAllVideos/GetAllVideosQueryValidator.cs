using FluentValidation;
using Stock_Exchange.Application.Localization;

namespace Stock_Exchange.Application.Features.Videos.Queries.GetAllVideos
{
    public class GetAllVideosQueryValidator : AbstractValidator<GetAllVideosQuery>
    {
        public GetAllVideosQueryValidator()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThan(0)
                .WithMessage("Page number must be greater than 0.");

            RuleFor(x => x.PageSize)
                .GreaterThan(0)
                .WithMessage("Page size must be greater than 0.")
                .LessThanOrEqualTo(100)
                .WithMessage("Page size must not exceed 100.");

            RuleFor(x => x.Search)
                .MaximumLength(100)
                .WithMessage(LocalizationKeys.VideoMessages.SearchTooLong)
                .When(x => !string.IsNullOrEmpty(x.Search));

            RuleFor(x => x.Category)
                .MaximumLength(100)
                .WithMessage(LocalizationKeys.VideoMessages.CategoryEnTooLong)
                .When(x => !string.IsNullOrEmpty(x.Category));
        }
    }
}
