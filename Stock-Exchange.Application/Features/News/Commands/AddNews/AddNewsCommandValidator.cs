using FluentValidation;

namespace Stock_Exchange.Application.Features.News.Commands.AddNews
{
    public class AddNewsCommandValidator : AbstractValidator<AddNewsCommand>
    {
        public AddNewsCommandValidator()
        {
            RuleFor(x => x.TitleEn)
                .NotEmpty().WithMessage("Title in English is required")
                .MaximumLength(500);

            RuleFor(x => x.TitleAr)
                .NotEmpty().WithMessage("Title in Arabic is required")
                .MaximumLength(500);

            RuleFor(x => x.SummaryEn)
                .MaximumLength(4000);

            RuleFor(x => x.SummaryAr)
                .MaximumLength(4000);

            RuleFor(x => x.CategoryEn)
                .MaximumLength(100);

            RuleFor(x => x.CategoryAr)
                .MaximumLength(100);

            RuleFor(x => x.ImageUrl)
                .MaximumLength(1024);

            RuleFor(x => x.DisplayOrder)
                .GreaterThanOrEqualTo(0);
        }
    }
}
