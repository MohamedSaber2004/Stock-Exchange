using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Common.Services;
using Stock_Exchange.Application.Localization;

namespace Stock_Exchange.Infrastructure.Services.Attachment
{
    public class ImageValidator : IImageValidator
    {
        private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp"];

        private readonly IBaseFileService _baseFileService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IStringLocalizer<Messages> _localizer;

        public ImageValidator(IBaseFileService baseFileService, IHttpClientFactory httpClientFactory, IStringLocalizer<Messages> localizer)
        {
            _baseFileService = baseFileService;
            _httpClientFactory = httpClientFactory;
            _localizer = localizer;
        }

        public async Task<(bool Uploaded, string Result)> UploadImage(IFormFile? file, int place)
        {
            if (!IsValidImage(file))
                return (false, _localizer[LocalizationKeys.Attachments.InvalidImageFormat].Value);

            var (uploaded, result) = await _baseFileService.UploadFileAsync(file, GetFolderPath(place));
            if (uploaded)
            {
                return (true, $"{place}_{Path.GetFileName(result)}");
            }
            return (false, result);
        }

        public async Task<(bool Uploaded, string Result)> UploadMultipleImage(List<IFormFile>? files, int place)
        {
            if (files == null || files.Count == 0)
                return (false, _localizer[LocalizationKeys.Attachments.NoMediaProvided].Value);

            var results = new List<string>();
            foreach (var file in files)
            {
                var (uploaded, result) = await UploadImage(file, place);
                if (uploaded)
                {
                    results.Add(result);
                }
            }

            if (results.Count == 0)
                return (false, _localizer[LocalizationKeys.Attachments.UploadFailed].Value);

            return (true, string.Join(",", results));
        }

        public bool ImageIsExisted(string? fullImagePath)
        {
            return _baseFileService.FileExists(fullImagePath);
        }

        public async Task<bool> DeleteImage(string? fileName, int place)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return false;

            if (fileName.Contains('_'))
            {
                var parts = fileName.Split('_');
                if (int.TryParse(parts[0], out _))
                {
                    fileName = string.Join("_", parts.Skip(1));
                }
            }
            return await _baseFileService.DeleteFileAsync(fileName, GetFolderPath(place));
        }

        public string GetUniqueFileName(string? fileName)
        {
            return _baseFileService.GetUniqueFileName(fileName);
        }

        public bool IsValidImage(IFormFile? file)
        {
            if (file == null || file.Length == 0 || string.IsNullOrEmpty(file.FileName))
                return false;

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            return AllowedExtensions.Contains(extension);
        }

        public bool IsValidImage(string? imageName, string? placeHolder)
        {
            return !string.IsNullOrWhiteSpace(imageName) && imageName != placeHolder;
        }

        public async Task<IFormFile?> ConvertImageToFormFile(string? imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl) || !Uri.TryCreate(imageUrl, UriKind.Absolute, out var uri))
                return null;

            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                using var response = await httpClient.GetAsync(uri);
                if (!response.IsSuccessStatusCode)
                    return null;

                var content = await response.Content.ReadAsByteArrayAsync();
                if (content.Length == 0)
                    return null;

                var stream = new MemoryStream(content);

                var fileName = Path.GetFileName(uri.LocalPath);
                if (string.IsNullOrWhiteSpace(fileName))
                    fileName = "downloaded_image.jpg";

                return new FormFile(stream, 0, content.Length, "file", fileName)
                {
                    Headers = new HeaderDictionary(),
                    ContentType = response.Content.Headers.ContentType?.ToString() ?? "application/octet-stream"
                };
            }
            catch
            {
                return null;
            }
        }

        private static string GetFolderPath(int place)
        {
            return UploadPaths.GetPath(place) ?? string.Empty;
        }
    }
}
