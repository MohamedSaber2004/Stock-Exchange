using FluentValidation;
using Stock_Exchange.Application.Localization;

namespace Stock_Exchange.Application.Features.News.Commands.UpdateNews
{
    public class UpdateNewsCommandValidator : AbstractValidator<UpdateNewsCommand>
    {
        public UpdateNewsCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage(LocalizationKeys.NewsMessages.IdRequired);

            RuleFor(x => x.TitleEn)
                .NotEmpty().WithMessage(LocalizationKeys.NewsMessages.TitleEnRequired)
                .MaximumLength(500).WithMessage(LocalizationKeys.NewsMessages.TitleEnTooLong);

            RuleFor(x => x.TitleAr)
                .NotEmpty().WithMessage(LocalizationKeys.NewsMessages.TitleArRequired)
                .MaximumLength(500).WithMessage(LocalizationKeys.NewsMessages.TitleArTooLong);

            RuleFor(x => x.SummaryEn)
                .MaximumLength(4000).WithMessage(LocalizationKeys.NewsMessages.SummaryEnTooLong)
                .When(x => !string.IsNullOrEmpty(x.SummaryEn));

            RuleFor(x => x.SummaryAr)
                .MaximumLength(4000).WithMessage(LocalizationKeys.NewsMessages.SummaryArTooLong)
                .When(x => !string.IsNullOrEmpty(x.SummaryAr));

            RuleFor(x => x.CategoryEn)
                .MaximumLength(100).WithMessage(LocalizationKeys.NewsMessages.CategoryEnTooLong)
                .When(x => !string.IsNullOrEmpty(x.CategoryEn));

            RuleFor(x => x.CategoryAr)
                .MaximumLength(100).WithMessage(LocalizationKeys.NewsMessages.CategoryArTooLong)
                .When(x => !string.IsNullOrEmpty(x.CategoryAr));

            RuleFor(x => x.ImageUrl)
                .MaximumLength(1024).WithMessage(LocalizationKeys.NewsMessages.ImageUrlTooLong)
                .When(x => !string.IsNullOrEmpty(x.ImageUrl));

            RuleFor(x => x.DisplayOrder)
                .GreaterThanOrEqualTo(0).WithMessage(LocalizationKeys.NewsMessages.DisplayOrderInvalid);
        }
    }
}
