using MediatR;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Domain.Enums;

namespace Stock_Exchange.Application.Features.Attachments.Commands.DownloadFile
{
    public record DownloadFileCommand : IRequest<Result<string>>
    {
        public string? FileName { get; set; }
        public int FilePlace { get; set; }
        public MediaType MediaType { get; set; }
    }
}
