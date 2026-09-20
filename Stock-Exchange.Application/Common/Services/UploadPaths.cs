using Microsoft.Extensions.Configuration;
using Stock_Exchange.Application.Common.Options;

namespace Stock_Exchange.Application.Common.Services
{
    public class UploadPaths
    {
        private static UploadPathsOptions? Options;

        public static void Configure(IConfiguration configuration)
        {
            Options = configuration.GetSection("UploadPaths").Get<UploadPathsOptions>();
        }

        public static string General => Options?.General ?? "General";

        public static string? GetPath(int place)
        {
            return place switch
            {
                0 => General,
                _ => string.Empty
            };
        }

        public static IEnumerable<string> GetAllPaths()
        {
            if (Options is null) yield break;
            yield return Options.General;
        }
    }
}
