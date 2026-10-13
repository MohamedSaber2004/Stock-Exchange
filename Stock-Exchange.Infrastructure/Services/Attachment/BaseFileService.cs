using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Localization;

namespace Stock_Exchange.Infrastructure.Services.Attachment
{
    public class BaseFileService : IBaseFileService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IStringLocalizer<Messages> _localizer;
        private readonly ILogger<BaseFileService> _logger;

        private string WebRootPath => _webHostEnvironment.WebRootPath ?? Path.Combine(_webHostEnvironment.ContentRootPath, "wwwroot");

        public BaseFileService(
            IWebHostEnvironment webHostEnvironment,
            IStringLocalizer<Messages> localizer,
            ILogger<BaseFileService> logger)
        {
            _webHostEnvironment = webHostEnvironment;
            _localizer = localizer;
            _logger = logger;
        }

        public async Task<(bool Uploaded, string Result)> UploadFileAsync(IFormFile? file, string folderPath)
        {
            if (file == null || file.Length == 0)
                return (false, _localizer[LocalizationKeys.Attachments.FileEmpty].Value);

            if (string.IsNullOrWhiteSpace(folderPath))
            {
                _logger.LogWarning("Upload file failed: folderPath is null or empty. Please check the requested place parameter or UploadPaths configuration.");
                return (false, _localizer[LocalizationKeys.Attachments.UploadFailed].Value);
            }

            try
            {
                string sanitizedFolder = folderPath.TrimStart('/', '\\');
                string uploadsFolder = Path.Combine(WebRootPath, sanitizedFolder);

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName = GetUniqueFileName(file.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                await using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                return (true, Path.Combine(sanitizedFolder, uniqueFileName).Replace("\\", "/"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Upload file failed due to exception: {Message}", ex.Message);
                return (false, _localizer[LocalizationKeys.Attachments.UploadFailed].Value);
            }
        }

        public bool FileExists(string? fullFilePath)
        {
            if (string.IsNullOrWhiteSpace(fullFilePath))
                return false;

            string filePath = Path.Combine(WebRootPath, fullFilePath.TrimStart('/', '\\'));
            return File.Exists(filePath);
        }

        public Task<bool> DeleteFileAsync(string fileName, string folderPath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(fileName) || string.IsNullOrWhiteSpace(folderPath))
                    return Task.FromResult(false);

                string safeFileName = Path.GetFileName(fileName);
                string filePath = Path.Combine(WebRootPath, folderPath.TrimStart('/', '\\'), safeFileName);

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    return Task.FromResult(true);
                }
                return Task.FromResult(false);
            }
            catch
            {
                return Task.FromResult(false);
            }
        }

        public string GetUniqueFileName(string? fileName)
        {
            var extension = string.IsNullOrEmpty(fileName) ? string.Empty : Path.GetExtension(fileName);
            return Guid.NewGuid().ToString() + extension;
        }

        public Task<(bool Success, string Result)> DownloadFileAsync(string folderPath, string fileName)
        {
            if (!string.IsNullOrWhiteSpace(folderPath) && !string.IsNullOrWhiteSpace(fileName))
            {
                string safeFileName = Path.GetFileName(fileName);
                string filePath = Path.Combine(WebRootPath, folderPath.TrimStart('/', '\\'), safeFileName);

                if (File.Exists(filePath))
                {
                    return Task.FromResult((true, filePath));
                }
            }

            return Task.FromResult((false, _localizer[LocalizationKeys.Attachments.FileNotFound].Value));
        }

        private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, SemaphoreSlim> _mergeLocks = new();

        public async Task<(bool Saved, string TempPath)> SaveChunkAsync(string uploadId, int chunkIndex, IFormFile chunk)
        {
            if (chunk == null || chunk.Length == 0)
                return (false, string.Empty);

            var safeUploadId = Path.GetFileName(uploadId);
            if (string.IsNullOrWhiteSpace(safeUploadId))
                return (false, string.Empty);

            try
            {
                var baseTempFolder = Path.Combine(WebRootPath, "temp_chunks");
                var tempFolder = Path.Combine(baseTempFolder, safeUploadId);
                if (!Directory.Exists(tempFolder))
                {
                    Directory.CreateDirectory(tempFolder);

                    // Background cleanup of abandoned uploads older than 12 hours to prevent disk quota exhaustion
                    _ = Task.Run(() =>
                    {
                        try
                        {
                            var baseDir = new DirectoryInfo(baseTempFolder);
                            if (baseDir.Exists)
                            {
                                foreach (var sub in baseDir.GetDirectories())
                                {
                                    if (sub.LastWriteTimeUtc < DateTime.UtcNow.AddHours(-12))
                                    {
                                        sub.Delete(true);
                                    }
                                }
                            }
                        }
                        catch { }
                    });
                }

                var chunkPath = Path.Combine(tempFolder, $"{chunkIndex}.part");
                await using (var fileStream = new FileStream(chunkPath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    await chunk.CopyToAsync(fileStream);
                }

                return (true, chunkPath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save chunk {ChunkIndex} for upload {UploadId}: {Message}", chunkIndex, uploadId, ex.Message);
                return (false, string.Empty);
            }
        }

        public bool AreAllChunksUploaded(string uploadId, int totalChunks)
        {
            var safeUploadId = Path.GetFileName(uploadId);
            if (string.IsNullOrWhiteSpace(safeUploadId)) return false;

            var tempFolder = Path.Combine(WebRootPath, "temp_chunks", safeUploadId);
            if (!Directory.Exists(tempFolder)) return false;

            for (int i = 0; i < totalChunks; i++)
            {
                if (!File.Exists(Path.Combine(tempFolder, $"{i}.part")))
                    return false;
            }
            return true;
        }

        public async Task<(bool Merged, string Result)> MergeChunksAsync(string uploadId, int totalChunks, string fileName, string folderPath)
        {
            var safeUploadId = Path.GetFileName(uploadId);
            if (string.IsNullOrWhiteSpace(safeUploadId))
                return (false, _localizer[LocalizationKeys.Attachments.UploadFailed].Value);

            var tempFolder = Path.Combine(WebRootPath, "temp_chunks", safeUploadId);
            if (!Directory.Exists(tempFolder))
                return (false, _localizer[LocalizationKeys.Attachments.UploadFailed].Value);

            var sem = _mergeLocks.GetOrAdd(safeUploadId, _ => new SemaphoreSlim(1, 1));
            await sem.WaitAsync();

            try
            {
                for (int i = 0; i < totalChunks; i++)
                {
                    var partPath = Path.Combine(tempFolder, $"{i}.part");
                    if (!File.Exists(partPath))
                    {
                        return (false, _localizer[LocalizationKeys.Attachments.UploadFailed].Value);
                    }
                }

                string sanitizedFolder = folderPath.TrimStart('/', '\\');
                string uploadsFolder = Path.Combine(WebRootPath, sanitizedFolder);
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName = GetUniqueFileName(fileName);
                string finalPath = Path.Combine(uploadsFolder, uniqueFileName);

                await using (var outputStream = new FileStream(finalPath, FileMode.Create, FileAccess.Write, FileShare.None, 65536, true))
                {
                    for (int i = 0; i < totalChunks; i++)
                    {
                        var partPath = Path.Combine(tempFolder, $"{i}.part");
                        await using (var partStream = new FileStream(partPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            await partStream.CopyToAsync(outputStream);
                        }
                    }
                }

                try
                {
                    Directory.Delete(tempFolder, true);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to delete temp chunk folder {TempFolder}: {Message}", tempFolder, ex.Message);
                }

                return (true, Path.Combine(sanitizedFolder, uniqueFileName).Replace("\\", "/"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to merge chunks for upload {UploadId}: {Message}", uploadId, ex.Message);
                return (false, _localizer[LocalizationKeys.Attachments.UploadFailed].Value);
            }
            finally
            {
                sem.Release();
                _mergeLocks.TryRemove(safeUploadId, out _);
            }
        }

        public bool DeleteTempChunks(string uploadId)
        {
            try
            {
                var safeUploadId = Path.GetFileName(uploadId);
                var tempFolder = Path.Combine(WebRootPath, "temp_chunks", safeUploadId);
                if (Directory.Exists(tempFolder))
                {
                    Directory.Delete(tempFolder, true);
                    return true;
                }
            }
            catch { }
            return false;
        }

        public string GetPhysicalPath(string relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
                return string.Empty;

            return Path.Combine(WebRootPath, relativePath.TrimStart('/', '\\').Replace('/', Path.DirectorySeparatorChar));
        }
    }
}
