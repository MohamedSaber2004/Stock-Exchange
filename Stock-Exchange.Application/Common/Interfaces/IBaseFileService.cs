using Microsoft.AspNetCore.Http;

namespace Stock_Exchange.Application.Common.Interfaces
{
    public interface IBaseFileService
    {
        Task<(bool Uploaded, string Result)> UploadFileAsync(IFormFile? file, string folderPath);
        bool FileExists(string? fullFilePath);
        Task<bool> DeleteFileAsync(string fileName, string folderPath);
        string GetUniqueFileName(string? fileName);
        Task<(bool Success, string Result)> DownloadFileAsync(string folderPath, string fileName);

        Task<(bool Saved, string TempPath)> SaveChunkAsync(string uploadId, int chunkIndex, IFormFile chunk);
        bool AreAllChunksUploaded(string uploadId, int totalChunks);
        Task<(bool Merged, string Result)> MergeChunksAsync(string uploadId, int totalChunks, string fileName, string folderPath);
        bool DeleteTempChunks(string uploadId);
    }
}
