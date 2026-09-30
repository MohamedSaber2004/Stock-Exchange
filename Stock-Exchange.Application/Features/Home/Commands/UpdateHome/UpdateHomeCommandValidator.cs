using FluentValidation;
using Stock_Exchange.Application.Localization;

namespace Stock_Exchange.Application.Features.Home.Commands.UpdateHome
{
    public class UpdateHomeCommandValidator : AbstractValidator<UpdateHomeCommand>
    {
        public UpdateHomeCommandValidator()
        {
            RuleFor(x => x.HeroTitleEn)
                .MaximumLength(500)
                .When(x => x.HeroTitleEn != null)
                .WithMessage(LocalizationKeys.HomeMessages.HeroTitleTooLong);

            RuleFor(x => x.HeroTitleAr)
                .MaximumLength(500)
                .When(x => x.HeroTitleAr != null)
                .WithMessage(LocalizationKeys.HomeMessages.HeroTitleTooLong);

            RuleFor(x => x.HeroSubtitleEn)
                .MaximumLength(2000)
                .When(x => x.HeroSubtitleEn != null)
                .WithMessage(LocalizationKeys.HomeMessages.HeroSubtitleTooLong);

            RuleFor(x => x.HeroSubtitleAr)
                .MaximumLength(2000)
                .When(x => x.HeroSubtitleAr != null)
                .WithMessage(LocalizationKeys.HomeMessages.HeroSubtitleTooLong);

            RuleFor(x => x.HeroImageUrl)
                .MaximumLength(1024)
                .When(x => x.HeroImageUrl != null)
                .WithMessage(LocalizationKeys.HomeMessages.HeroImageUrlTooLong);
        }
    }
}
