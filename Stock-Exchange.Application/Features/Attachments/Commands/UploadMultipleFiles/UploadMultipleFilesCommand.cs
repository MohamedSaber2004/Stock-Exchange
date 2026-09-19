using MediatR;
using Microsoft.AspNetCore.Http;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Domain.Enums;

namespace Stock_Exchange.Application.Features.Attachments.Commands.UploadMultipleFiles
{
    public record UploadMultipleFilesCommand : IRequest<Result<string>>
    {
        public List<IFormFile>? Files { get; set; }
        public MediaType MediaType { get; set; }
        public int Place { get; set; }
    }
}
