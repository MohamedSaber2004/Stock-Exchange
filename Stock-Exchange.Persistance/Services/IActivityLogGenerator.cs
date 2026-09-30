using Microsoft.EntityFrameworkCore.ChangeTracking;
using Stock_Exchange.Domain.Entities;

namespace Stock_Exchange.Persistance.Services
{
    public interface IActivityLogGenerator
    {
        List<ActivityLog> GenerateActivityLogs(ChangeTracker changeTracker);
    }
}
