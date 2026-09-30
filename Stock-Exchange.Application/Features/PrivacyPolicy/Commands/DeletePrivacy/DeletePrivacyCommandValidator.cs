using FluentValidation;

namespace Stock_Exchange.Application.Features.PrivacyPolicy.Commands.DeletePrivacy
{
    public class DeletePrivacyCommandValidator : AbstractValidator<DeletePrivacyCommand>
    {
        public DeletePrivacyCommandValidator()
        {
            RuleFor(x => x.Id)
                .Must(id => !id.HasValue || id.Value != Guid.Empty)
                .WithMessage("Id cannot be empty when provided.");
        }
    }
}
