namespace Stock_Exchange.Application.Common.Interfaces
{
    public interface IVideoCompressorService
    {
        Task<(bool Success, string FinalFilePath)> CompressVideoAsync(string absoluteFilePath, CancellationToken cancellationToken = default);
    }
}