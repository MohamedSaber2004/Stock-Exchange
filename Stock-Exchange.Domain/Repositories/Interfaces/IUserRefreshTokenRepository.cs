using Stock_Exchange.Domain.Entities;
using Stock_Exchange.Domain.Repositories.Interfaces.Base;

namespace Stock_Exchange.Domain.Repositories.Interfaces
{
    public interface IUserRefreshTokenRepository : IGenericRepository<UserRefreshToken, Guid>
    {
    }
}
