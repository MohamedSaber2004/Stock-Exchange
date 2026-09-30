using Stock_Exchange.Application.Common.Extensions;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Domain.Enums;
using System.Globalization;

namespace Stock_Exchange.Services
{
    public class CurrentLanguageService : ICurrentLanguageService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentLanguageService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Language Language => AppLanguageExtensions.FromCode(ResolveCulture());

        public string LanguageCode => Language.ToCode();

        private string? ResolveCulture()
        {
            var request = _httpContextAccessor.HttpContext?.Request;
            if (request is null)
                return CultureInfo.CurrentUICulture?.Name;

            // 1. Explicit query parameter override (e.g. ?lang=en, ?lang=ar, ?culture=en)
            var queryCulture = request.Query["lang"].FirstOrDefault()
                               ?? request.Query["language"].FirstOrDefault()
                               ?? request.Query["culture"].FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(queryCulture))
                return queryCulture;

            // 2. Explicit custom headers or standard Accept-Language header
            var headers = request.Headers;

            var headerCulture = headers["Language"].FirstOrDefault()
                                ?? headers["Lang"].FirstOrDefault()
                                ?? headers["Culture"].FirstOrDefault()
                                ?? headers["Accept-Language"].FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(headerCulture))
                return headerCulture;

            return CultureInfo.CurrentUICulture?.Name;
        }
    }
}
