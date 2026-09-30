using FluentValidation;
using Stock_Exchange.Application.Features.Home.DTOs;

namespace Stock_Exchange.Application.Features.Home.Commands.UpdateHomeArticles
{
    public class UpdateHomeArticlesCommandValidator : AbstractValidator<UpdateHomeArticlesCommand>
    {
        public UpdateHomeArticlesCommandValidator()
        {
            RuleForEach(x => x.Items).SetValidator(new HomeArticleItemRequestValidator());
        }
    }

    public class HomeArticleItemRequestValidator : AbstractValidator<HomeArticleItemRequest>
    {
        public HomeArticleItemRequestValidator()
        {
            RuleFor(x => x.TitleEn)
                .NotEmpty().WithMessage("Article English title is required.")
                .MaximumLength(500).WithMessage("Article English title must not exceed 500 characters.");

            RuleFor(x => x.TitleAr)
                .NotEmpty().WithMessage("Article Arabic title is required.")
                .MaximumLength(500).WithMessage("Article Arabic title must not exceed 500 characters.");

            RuleFor(x => x.ExcerptEn)
                .MaximumLength(2000).WithMessage("Article English excerpt must not exceed 2000 characters.");

            RuleFor(x => x.ExcerptAr)
                .MaximumLength(2000).WithMessage("Article Arabic excerpt must not exceed 2000 characters.");

            RuleFor(x => x.AuthorName)
                .MaximumLength(200).WithMessage("Author name must not exceed 200 characters.");

            RuleFor(x => x.ReadMinutes)
                .GreaterThanOrEqualTo(0).WithMessage("Read minutes must be non-negative.");

            RuleFor(x => x.ImageUrl)
                .MaximumLength(1024).WithMessage("Image URL must not exceed 1024 characters.");
        }
    }
}
