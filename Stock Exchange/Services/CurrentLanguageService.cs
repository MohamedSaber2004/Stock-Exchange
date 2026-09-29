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

            var headers = request.Headers;

            var headerCulture = headers["Accept-Language"].FirstOrDefault()
                                ?? headers["Language"].FirstOrDefault()
                                ?? headers["Culture"].FirstOrDefault()
                                ?? headers["Lang"].FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(headerCulture))
                return headerCulture;

            var queryCulture = request.Query["culture"].FirstOrDefault()
                               ?? request.Query["lang"].FirstOrDefault()
                               ?? request.Query["language"].FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(queryCulture))
                return queryCulture;

            return CultureInfo.CurrentUICulture?.Name;
        }
    }
}
