using FluentValidation;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Localization;

namespace Stock_Exchange.Application.Features.Auth.Commands.DeleteAccount
{
    public class DeleteAccountCommandValidator : AbstractValidator<DeleteAccountCommand>
    {
        public DeleteAccountCommandValidator(ICurrentUserService currentUserService)
        {
            RuleFor(x => x)
                .Must(_ => currentUserService.IsAuthenticated && currentUserService.UserId != Guid.Empty)
                .WithMessage(LocalizationKeys.ExceptionMessages.Unauthorized);
        }
    }
}
