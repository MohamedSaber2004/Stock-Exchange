using Stock_Exchange.Domain.Entities;
using Stock_Exchange.Domain.Repositories.Interfaces;
using Stock_Exchange.Infrastructure.Repositories.Implementations.Base;
using Stock_Exchange.Persistance;

namespace Stock_Exchange.Infrastructure.Repositories.Implementations
{
    public class HomeRepository : GenericRepository<Home, Guid>, IHomeRepository
    {
        public HomeRepository(StockExchangeDbContext context) : base(context)
        {
        }
    }
}
