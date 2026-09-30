using FluentValidation;
using Stock_Exchange.Application.Features.Home.DTOs;
using Stock_Exchange.Application.Localization;

namespace Stock_Exchange.Application.Features.Home.Commands.UpdateHomeNews
{
    public class UpdateHomeNewsCommandValidator : AbstractValidator<UpdateHomeNewsCommand>
    {
        public UpdateHomeNewsCommandValidator()
        {
            RuleForEach(x => x.Items).SetValidator(new HomeNewsItemRequestValidator());
        }
    }

    public class HomeNewsItemRequestValidator : AbstractValidator<HomeNewsItemRequest>
    {
        public HomeNewsItemRequestValidator()
        {
            RuleFor(x => x.TitleEn)
                .NotEmpty().WithMessage(LocalizationKeys.HomeMessages.HomeNewsTitleEnRequired)
                .MaximumLength(500).WithMessage(LocalizationKeys.HomeMessages.HomeNewsTitleEnTooLong);

            RuleFor(x => x.TitleAr)
                .NotEmpty().WithMessage(LocalizationKeys.HomeMessages.HomeNewsTitleArRequired)
                .MaximumLength(500).WithMessage(LocalizationKeys.HomeMessages.HomeNewsTitleArTooLong);

            RuleFor(x => x.SummaryEn)
                .MaximumLength(4000).WithMessage(LocalizationKeys.HomeMessages.HomeNewsSummaryEnTooLong);

            RuleFor(x => x.SummaryAr)
                .MaximumLength(4000).WithMessage(LocalizationKeys.HomeMessages.HomeNewsSummaryArTooLong);

            RuleFor(x => x.CategoryEn)
                .MaximumLength(200).WithMessage(LocalizationKeys.HomeMessages.HomeNewsCategoryEnTooLong);

            RuleFor(x => x.CategoryAr)
                .MaximumLength(200).WithMessage(LocalizationKeys.HomeMessages.HomeNewsCategoryArTooLong);

            RuleFor(x => x.ImageUrl)
                .MaximumLength(1024).WithMessage(LocalizationKeys.HomeMessages.HomeNewsImageUrlTooLong);
        }
    }
}
