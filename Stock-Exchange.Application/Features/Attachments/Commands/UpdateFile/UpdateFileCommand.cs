using MediatR;
using Microsoft.AspNetCore.Http;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Domain.Enums;

namespace Stock_Exchange.Application.Features.Attachments.Commands.UpdateFile
{
    public record UpdateFileCommand : IRequest<Result<string>>
    {
        public string? OldFileName { get; set; }
        public IFormFile? File { get; set; }
        public MediaType MediaType { get; set; }
        public int Place { get; set; }
    }
}
