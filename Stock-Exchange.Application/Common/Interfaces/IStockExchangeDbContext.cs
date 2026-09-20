using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Stock_Exchange.Application.Common.Interfaces
{
    public interface IClinicHubContext : IAsyncDisposable
    {
        DbSet<IdentityUserRole<Guid>> UserRoles { get; }
        DbSet<IdentityRole<Guid>> Roles { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
