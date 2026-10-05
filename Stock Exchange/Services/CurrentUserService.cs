using Stock_Exchange.Application.Common.Interfaces;
using System.Net;
using System.Security.Claims;

namespace Stock_Exchange.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

        public Guid UserId
        {
            get
            {
                var userIdString = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                    ?? User?.FindFirst("sub")?.Value;

                return Guid.TryParse(userIdString, out var userId)
                    ? userId
                    : Guid.Empty;
            }
        }

        public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

        public string? IpAddress
        {
            get
            {
                var context = _httpContextAccessor.HttpContext;
                if (context == null) return "127.0.0.1";

                // 1. Explicit Client IP header (sent by frontend interceptor or custom proxy)
                var clientIp = CleanAndValidateIp(context.Request.Headers["X-Client-IP"].FirstOrDefault());
                if (!string.IsNullOrEmpty(clientIp)) return clientIp;

                // 2. Cloudflare Connecting IP (used on MonsterASP.NET / RunASP.net / Cloudflare CDN)
                var cfIp = CleanAndValidateIp(context.Request.Headers["CF-Connecting-IP"].FirstOrDefault());
                if (!string.IsNullOrEmpty(cfIp)) return cfIp;

                // 3. Vercel & Nginx Real IP headers
                var realIp = CleanAndValidateIp(context.Request.Headers["X-Real-IP"].FirstOrDefault())
                             ?? CleanAndValidateIp(context.Request.Headers["X-Vercel-Forwarded-For"].FirstOrDefault())
                             ?? CleanAndValidateIp(context.Request.Headers["True-Client-IP"].FirstOrDefault());
                if (!string.IsNullOrEmpty(realIp)) return realIp;

                // 4. X-Forwarded-For (standard multi-proxy chain)
                var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(forwardedFor))
                {
                    var ips = forwardedFor.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    foreach (var candidate in ips)
                    {
                        var cleaned = CleanAndValidateIp(candidate);
                        if (!string.IsNullOrEmpty(cleaned))
                        {
                            return cleaned;
                        }
                    }
                }

                // 5. Connection Remote IP Address (handles IPv4 mapped to IPv6 ::ffff:x.x.x.x)
                var connectionIp = context.Connection.RemoteIpAddress;
                if (connectionIp != null)
                {
                    var cleaned = CleanAndValidateIp(connectionIp.ToString());
                    if (!string.IsNullOrEmpty(cleaned)) return cleaned;
                }

                return "127.0.0.1";
            }
        }

        private static string? CleanAndValidateIp(string? rawIp)
        {
            if (string.IsNullOrWhiteSpace(rawIp)) return null;

            var trimmed = rawIp.Trim();
            if (trimmed.Equals("unknown", StringComparison.OrdinalIgnoreCase)) return null;

            // Strip IPv6 brackets and port: [2001:db8::1]:8080 -> 2001:db8::1
            if (trimmed.StartsWith("[") && trimmed.Contains("]"))
            {
                var closingBracket = trimmed.IndexOf(']');
                trimmed = trimmed.Substring(1, closingBracket - 1);
            }
            // Strip IPv4 port: 192.168.1.1:8080 -> 192.168.1.1 (contains single colon)
            else if (trimmed.Contains(':') && !trimmed.Contains("::") && trimmed.IndexOf(':') == trimmed.LastIndexOf(':'))
            {
                trimmed = trimmed.Split(':')[0];
            }

            if (IPAddress.TryParse(trimmed, out var ip))
            {
                // Unmap IPv4 mapped into IPv6 (e.g. ::ffff:192.168.1.1 -> 192.168.1.1)
                if (ip.IsIPv4MappedToIPv6)
                {
                    ip = ip.MapToIPv4();
                }

                // Treat loopback addresses consistently as 127.0.0.1
                if (IPAddress.IsLoopback(ip) || ip.ToString() == "::1")
                {
                    return "127.0.0.1";
                }

                return ip.ToString();
            }

            return null;
        }

        public string CorrelationId => _httpContextAccessor.HttpContext?.TraceIdentifier ?? string.Empty;

        public string? Email => User?.FindFirst(ClaimTypes.Email)?.Value
            ?? User?.FindFirst("email")?.Value;

        public string? FullName => User?.FindFirst("FullName")?.Value
            ?? User?.FindFirst(ClaimTypes.Name)?.Value;

        public string? Device
        {
            get
            {
                var userAgent = _httpContextAccessor.HttpContext?.Request.Headers["User-Agent"].ToString();
                return ParseUserAgent(userAgent);
            }
        }

        public int? UserTypes
        {
            get
            {
                var userTypesClaim = User?.FindFirst("UserTypes")?.Value;
                return int.TryParse(userTypesClaim, out var userTypes)
                    ? userTypes
                    : null;
            }
        }

        private static string ParseUserAgent(string? userAgent)
        {
            if (string.IsNullOrWhiteSpace(userAgent))
                return "Unknown Device";

            var browser = "Browser";
            if (userAgent.Contains("Edg/", StringComparison.OrdinalIgnoreCase))
                browser = "Edge";
            else if (userAgent.Contains("Chrome", StringComparison.OrdinalIgnoreCase) && !userAgent.Contains("Edg", StringComparison.OrdinalIgnoreCase))
                browser = "Chrome";
            else if (userAgent.Contains("Safari", StringComparison.OrdinalIgnoreCase) && !userAgent.Contains("Chrome", StringComparison.OrdinalIgnoreCase))
                browser = "Safari";
            else if (userAgent.Contains("Firefox", StringComparison.OrdinalIgnoreCase))
                browser = "Firefox";

            var os = string.Empty;
            if (userAgent.Contains("Windows NT 10.0", StringComparison.OrdinalIgnoreCase))
                os = "Windows 11/10";
            else if (userAgent.Contains("Windows", StringComparison.OrdinalIgnoreCase))
                os = "Windows";
            else if (userAgent.Contains("iPhone", StringComparison.OrdinalIgnoreCase))
                os = "iPhone";
            else if (userAgent.Contains("iPad", StringComparison.OrdinalIgnoreCase))
                os = "iPad";
            else if (userAgent.Contains("Macintosh", StringComparison.OrdinalIgnoreCase) || userAgent.Contains("Mac OS", StringComparison.OrdinalIgnoreCase))
                os = "macOS";
            else if (userAgent.Contains("Android", StringComparison.OrdinalIgnoreCase))
                os = "Android";
            else if (userAgent.Contains("Linux", StringComparison.OrdinalIgnoreCase))
                os = "Linux";

            return string.IsNullOrEmpty(os) ? browser : $"{browser} on {os}";
        }
    }
}