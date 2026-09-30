using Stock_Exchange.Domain.Entities;
using Stock_Exchange.Domain.Repositories.Interfaces;
using Stock_Exchange.Infrastructure.Repositories.Implementations.Base;
using Stock_Exchange.Persistance;

namespace Stock_Exchange.Infrastructure.Repositories.Implementations
{
    public class NewsRepository : GenericRepository<News, Guid>, INewsRepository
    {
        public NewsRepository(StockExchangeDbContext context) : base(context)
        {
        }
    }
}
