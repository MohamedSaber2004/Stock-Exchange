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
                .WithMessage(LocalizationKeys.VideoMessages.PageNumberInvalid);

            RuleFor(x => x.PageSize)
                .GreaterThan(0)
                .WithMessage(LocalizationKeys.VideoMessages.PageSizeInvalid)
                .LessThanOrEqualTo(100)
                .WithMessage(LocalizationKeys.VideoMessages.PageSizeTooLarge);

            RuleFor(x => x.Search)
                .MaximumLength(100)
                .WithMessage(LocalizationKeys.VideoMessages.SearchTooLong)
                .When(x => !string.IsNullOrEmpty(x.Search));
        }
    }
}
