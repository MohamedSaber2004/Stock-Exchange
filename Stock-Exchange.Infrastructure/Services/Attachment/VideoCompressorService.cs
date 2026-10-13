using FFMpegCore;
using FFMpegCore.Enums;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Common.Options;

namespace Stock_Exchange.Infrastructure.Services.Attachment
{
    public class VideoCompressorService : IVideoCompressorService
    {
        private readonly VideoCompressionOptions _options;
        private readonly ILogger<VideoCompressorService> _logger;
        private readonly IWebHostEnvironment _env;

        public VideoCompressorService(
            IOptions<VideoCompressionOptions> options,
            ILogger<VideoCompressorService> logger,
            IWebHostEnvironment env)
        {
            _options = options.Value;
            _logger = logger;
            _env = env;

            ConfigureFFmpeg();
        }

        private void ConfigureFFmpeg()
        {
            if (!string.IsNullOrWhiteSpace(_options.FfmpegBinaryFolder) && Directory.Exists(_options.FfmpegBinaryFolder))
            {
                GlobalFFOptions.Configure(new FFOptions { BinaryFolder = _options.FfmpegBinaryFolder });
                return;
            }

            var localTools = Path.Combine(_env.ContentRootPath, "tools", "ffmpeg");
            if (Directory.Exists(localTools))
            {
                GlobalFFOptions.Configure(new FFOptions { BinaryFolder = localTools });
            }
        }

        public async Task<(bool Success, string FinalFilePath)> CompressVideoAsync(string absoluteFilePath, CancellationToken cancellationToken = default)
        {
            if (!_options.Enabled)
                return (true, absoluteFilePath);

            if (string.IsNullOrWhiteSpace(absoluteFilePath) || !File.Exists(absoluteFilePath))
                return (false, absoluteFilePath);

            var originalFileInfo = new FileInfo(absoluteFilePath);
            if (originalFileInfo.Length == 0)
                return (false, absoluteFilePath);

            var dir = Path.GetDirectoryName(absoluteFilePath)!;
            var tempOutput = Path.Combine(dir, $"{Guid.NewGuid()}_compressed.mp4");

            try
            {
                var mediaInfo = await FFProbe.AnalyseAsync(absoluteFilePath, cancellationToken: cancellationToken);
                var videoStream = mediaInfo.PrimaryVideoStream;
                if (videoStream == null)
                {
                    _logger.LogWarning("Video compression skipped: No primary video stream found for {Path}", absoluteFilePath);
                    return (true, absoluteFilePath);
                }

                var speed = ParseSpeed(_options.Preset);

                var ffmpegArgs = FFMpegArguments
                    .FromFileInput(absoluteFilePath)
                    .OutputToFile(tempOutput, true, options =>
                    {
                        options
                            .WithVideoCodec(VideoCodec.LibX264)
                            .WithConstantRateFactor(_options.Crf)
                            .WithSpeedPreset(speed)
                            .WithAudioCodec(AudioCodec.Aac)
                            .WithAudioBitrate(_options.AudioBitrateKbps)
                            .WithFastStart();

                        if (_options.MaxHeight > 0 && videoStream.Height > _options.MaxHeight)
                        {
                            options.Resize(-2, _options.MaxHeight);
                        }
                    });

                await ffmpegArgs.ProcessAsynchronously(throwOnError: true);

                if (!File.Exists(tempOutput))
                {
                    _logger.LogWarning("Video compression produced no output file for {Path}. Keeping original.", absoluteFilePath);
                    return (true, absoluteFilePath);
                }

                var compressedFileInfo = new FileInfo(tempOutput);

                // If compressed file is smaller, replace or rename
                if (compressedFileInfo.Length < originalFileInfo.Length)
                {
                    var targetFinalPath = Path.ChangeExtension(absoluteFilePath, ".mp4");
                    if (string.Equals(targetFinalPath, absoluteFilePath, StringComparison.OrdinalIgnoreCase))
                    {
                        File.Delete(absoluteFilePath);
                        File.Move(tempOutput, absoluteFilePath);
                        _logger.LogInformation("Video compressed successfully: {OriginalSize} bytes -> {NewSize} bytes", originalFileInfo.Length, compressedFileInfo.Length);
                        return (true, absoluteFilePath);
                    }
                    else
                    {
                        if (File.Exists(targetFinalPath))
                        {
                            File.Delete(targetFinalPath);
                        }
                        File.Move(tempOutput, targetFinalPath);
                        File.Delete(absoluteFilePath);
                        _logger.LogInformation("Video compressed and converted to MP4: {OriginalSize} bytes -> {NewSize} bytes", originalFileInfo.Length, compressedFileInfo.Length);
                        return (true, targetFinalPath);
                    }
                }
                else
                {
                    // Output is larger or equal, keep original
                    _logger.LogInformation("Compressed video ({CompSize} bytes) was not smaller than original ({OrigSize} bytes). Retaining original.", compressedFileInfo.Length, originalFileInfo.Length);
                    File.Delete(tempOutput);
                    return (true, absoluteFilePath);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Video compression failed for {Path}. Retaining original file. Error: {Message}", absoluteFilePath, ex.Message);
                if (File.Exists(tempOutput))
                {
                    try { File.Delete(tempOutput); } catch { }
                }
                return (true, absoluteFilePath);
            }
        }

        private static Speed ParseSpeed(string? preset)
        {
            return preset?.ToLowerInvariant() switch
            {
                "ultrafast" => Speed.UltraFast,
                "superfast" => Speed.SuperFast,
                "veryfast" => Speed.VeryFast,
                "faster" => Speed.Faster,
                "fast" => Speed.Fast,
                "medium" => Speed.Medium,
                "slow" => Speed.Slow,
                "slower" => Speed.Slower,
                "veryslow" => Speed.VerySlow,
                _ => Speed.VeryFast
            };
        }
    }
}