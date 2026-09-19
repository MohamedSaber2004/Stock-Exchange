using Stock_Exchange.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stock_Exchange.Application.Common.Extensions
{
    public static class AppLanguageExtensions
    {
        public const string EnglishCode = "en";
        public const string ArabicCode = "ar";

        public static string ToCode(this Language language) => language switch
        {
            Language.ar => ArabicCode,
            Language.en => EnglishCode,
            _ => EnglishCode
        };

        public static Language FromCode(string? code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return Language.en;
            }

            var parts = code.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            foreach (var part in parts)
            {
                var langPart = part.Split(';')[0].Trim().ToLowerInvariant();

                if (langPart.StartsWith("ar"))
                {
                    return Language.ar;
                }

                if (langPart.StartsWith("en"))
                {
                    return Language.en;
                }
            }

            return Language.en;
        }

        public static string[] GetAllCodes() => new[]
        {
            Language.en.ToCode(),
            Language.ar.ToCode()
        };
    }
}
