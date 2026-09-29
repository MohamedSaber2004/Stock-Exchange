using FluentValidation;
using Stock_Exchange.Application.Localization;
using Stock_Exchange.Domain.Entities;
using Stock_Exchange.Domain.Repositories.Interfaces.Base;

namespace Stock_Exchange.Application.Features.HelpCenter.Queries.GetAllHelpCenters
{
    public class GetAllHelpCentersQueryValidator : AbstractValidator<GetAllHelpCentersQuery>
    {
        private readonly IUnitOfWork _ctx;

        public GetAllHelpCentersQueryValidator(IUnitOfWork ctx)
        {
            _ctx = ctx;

            RuleFor(x => x.CategoryId)
                .NotEqual(Guid.Empty)
                .WithMessage(LocalizationKeys.HelpCenterMessages.InvalidCategoryId);

            RuleFor(x => x.CategoryId)
                .MustAsync((categoryId, cancellationToken) =>
                    categoryId.HasValue && categoryId.Value != Guid.Empty
                        ? CategoryExists(categoryId.Value, cancellationToken)
                        : Task.FromResult(true))
                .WithMessage(LocalizationKeys.HelpCenterMessages.CategoryNotFound);

            RuleFor(x => x.Search)
                .MaximumLength(200)
                .WithMessage(LocalizationKeys.HelpCenterMessages.SearchTooLong);
        }

        private Task<bool> CategoryExists(Guid categoryId, CancellationToken cancellationToken)
        {
            return _ctx.GetRepository<HelpCenterCategory, Guid>()
                .ExistsAsync(c => c.Id == categoryId && !c.IsDeleted && c.IsActive, cancellationToken);
        }
    }
}
