using FluentValidation;
using Stock_Exchange.Application.Common.Services;
using Stock_Exchange.Application.Localization;

namespace Stock_Exchange.Application.Features.Attachments.Commands.DownloadFile
{
    public class DownloadFileCommandValidator : AbstractValidator<DownloadFileCommand>
    {
        public DownloadFileCommandValidator()
        {
            RuleFor(x => x.FileName)
                .NotEmpty()
                .WithMessage(LocalizationKeys.Attachments.FileNotFound);

            RuleFor(x => x.FilePlace)
                .GreaterThanOrEqualTo(0)
                .Must(place => !string.IsNullOrWhiteSpace(UploadPaths.GetPath(place)))
                .WithMessage(LocalizationKeys.ExceptionMessages.BadRequest);
        }
    }
}
