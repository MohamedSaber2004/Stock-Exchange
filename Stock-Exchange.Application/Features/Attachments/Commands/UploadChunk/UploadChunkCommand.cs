using MediatR;
using Microsoft.AspNetCore.Http;
using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Domain.Enums;

namespace Stock_Exchange.Application.Features.Attachments.Commands.UploadChunk
{
    public record UploadChunkCommand : IRequest<Result<UploadChunkResultDto>>
    {
        public string UploadId { get; set; } = string.Empty;
        public int ChunkIndex { get; set; }
        public int TotalChunks { get; set; }
        public string FileName { get; set; } = string.Empty;
        public MediaType MediaType { get; set; }
        public int Place { get; set; }
        public IFormFile? Chunk { get; set; }
    }
}
