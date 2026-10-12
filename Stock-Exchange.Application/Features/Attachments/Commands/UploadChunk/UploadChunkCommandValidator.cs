using FluentValidation;
using Stock_Exchange.Application.Common.Services;
using Stock_Exchange.Application.Localization;

namespace Stock_Exchange.Application.Features.Attachments.Commands.UploadChunk
{
    public class UploadChunkCommandValidator : AbstractValidator<UploadChunkCommand>
    {
        public UploadChunkCommandValidator()
        {
            RuleFor(x => x.UploadId)
                .NotEmpty()
                .WithMessage(LocalizationKeys.ExceptionMessages.BadRequest);

            RuleFor(x => x.TotalChunks)
                .GreaterThan(0)
                .WithMessage(LocalizationKeys.ExceptionMessages.BadRequest);

            RuleFor(x => x.ChunkIndex)
                .GreaterThanOrEqualTo(0)
                .LessThan(x => x.TotalChunks)
                .WithMessage(LocalizationKeys.ExceptionMessages.BadRequest);

            RuleFor(x => x.FileName)
                .NotEmpty()
                .WithMessage(LocalizationKeys.Attachments.FileEmpty);

            RuleFor(x => x.Chunk)
                .NotNull()
                .WithMessage(LocalizationKeys.Attachments.FileEmpty);

            RuleFor(x => x.Place)
                .GreaterThanOrEqualTo(0)
                .Must(place => !string.IsNullOrWhiteSpace(UploadPaths.GetPath(place)))
                .WithMessage(LocalizationKeys.ExceptionMessages.BadRequest);
        }
    }
}
