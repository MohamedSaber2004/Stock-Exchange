namespace Stock_Exchange.Application.Common.Options
{
    public class VideoCompressionOptions
    {
        public bool Enabled { get; set; } = true;
        public string? FfmpegBinaryFolder { get; set; }
        public int Crf { get; set; } = 27;
        public string Preset { get; set; } = "veryfast";
        public int MaxHeight { get; set; } = 1080;
        public int AudioBitrateKbps { get; set; } = 128;
    }
}