using Stock_Exchange.Domain.Entities;
using Stock_Exchange.Domain.Enums;

namespace Stock_Exchange.Application.Common.Interfaces
{
    public interface IActivityLogService
    {
        Task<ActivityLog> LogAsync(
            string action,
            ActivityResourceType resourceType,
            Guid? userId = null,
            string? userEmail = null,
            string? details = null,
            string? actionEn = null,
            string? detailsEn = null,
            CancellationToken cancellationToken = default);
    }
}
