namespace Stock_Exchange.Application.Features.Attachments.Commands.UploadChunk
{
    public class UploadChunkResultDto
    {
        public bool IsCompleted { get; set; }
        public string? StoredFileName { get; set; }
        public int ChunkIndex { get; set; }
        public int TotalChunks { get; set; }
    }
}
