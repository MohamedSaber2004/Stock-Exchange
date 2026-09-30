using FluentValidation;
using Stock_Exchange.Application.Localization;

namespace Stock_Exchange.Application.Features.Videos.Commands.DeleteVideo
{
    public class DeleteVideoCommandValidator : AbstractValidator<DeleteVideoCommand>
    {
        public DeleteVideoCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage(LocalizationKeys.VideoMessages.IdRequired);
        }
    }
}
