using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Domain.Entities;
using Stock_Exchange.Domain.Enums;
using Stock_Exchange.Domain.Repositories.Interfaces;
using Stock_Exchange.Domain.Repositories.Interfaces.Base;

namespace Stock_Exchange.Infrastructure.Services
{
    public class ActivityLogService : IActivityLogService
    {
        private readonly IActivityLogRepository _activityLogRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICurrentUserService _currentUserService;

        public ActivityLogService(
            IActivityLogRepository activityLogRepository,
            IUnitOfWork unitOfWork,
            IHttpContextAccessor httpContextAccessor,
            ICurrentUserService currentUserService)
        {
            _activityLogRepository = activityLogRepository;
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _currentUserService = currentUserService;
        }

        public async Task<ActivityLog> LogAsync(
            string action,
            ActivityResourceType resourceType,
            Guid? userId = null,
            string? userEmail = null,
            string? details = null,
            string? actionEn = null,
            string? detailsEn = null,
            string? userName = null,
            CancellationToken cancellationToken = default)
        {
            var ipAddress = _currentUserService.IpAddress ?? string.Empty;
            var targetUserId = userId ?? (_currentUserService.IsAuthenticated ? _currentUserService.UserId : (Guid?)null);
            var targetUserName = userName ?? (_currentUserService.IsAuthenticated ? _currentUserService.FullName : null) ?? string.Empty;
            var targetUserEmail = userEmail ?? (_currentUserService.IsAuthenticated ? _currentUserService.Email : null) ?? string.Empty;
            var userAgent = _httpContextAccessor.HttpContext?.Request.Headers["User-Agent"].ToString();
            var device = ParseUserAgent(userAgent);

            var count = await _activityLogRepository.GetAllAsync(null).CountAsync(cancellationToken);
            var formattedId = $"act-{(count + 1):D3}";

            var log = new ActivityLog
            {
                FormattedId = formattedId,
                UserId = targetUserId,
                UserName = targetUserName,
                UserEmail = targetUserEmail,
                Action = action ?? string.Empty,
                ActionAr = action ?? string.Empty,
                ActionEn = actionEn ?? action ?? string.Empty,
                ResourceType = resourceType,
                IpAddress = ipAddress,
                Device = device,
                Details = details,
                DetailsAr = details,
                DetailsEn = detailsEn ?? details
            };

            log.MarkAsCreated(targetUserId?.ToString() ?? "System", DateTime.UtcNow);

            await _activityLogRepository.AddAsync(log);
            await _unitOfWork.SaveChangesAsync();

            return log;
        }

        private static string ParseUserAgent(string? userAgent)
        {
            if (string.IsNullOrWhiteSpace(userAgent))
                return string.Empty;

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
