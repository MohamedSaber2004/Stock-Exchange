using FluentValidation;
using Stock_Exchange.Application.Localization;

namespace Stock_Exchange.Application.Features.Videos.Queries.GetVideoById
{
    public class GetVideoByIdQueryValidator : AbstractValidator<GetVideoByIdQuery>
    {
        public GetVideoByIdQueryValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage(LocalizationKeys.VideoMessages.IdRequired);
        }
    }
}
