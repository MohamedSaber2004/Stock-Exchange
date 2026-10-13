using Stock_Exchange.Application.Common.Models;
using Stock_Exchange.Domain.Entities;

namespace Stock_Exchange.Application.Common.Interfaces
{
    public interface IUserPurgeService
    {
        Task<Result<bool>> PurgeUserAsync(ApplicationUser user, CancellationToken cancellationToken);
    }
}
