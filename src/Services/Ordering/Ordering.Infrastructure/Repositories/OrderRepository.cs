using Microsoft.EntityFrameworkCore;
using Ordering.Application.Contracts.Persistence;
using Ordering.Domain.Entities;
using Ordering.Infrastructure.Persistence;

namespace Ordering.Infrastructure.Repositories
{
    public class OrderRepository : RepositoryBase<Order>, IOrderRepository
    {
        public OrderRepository(OrderContext context) 
            : base(context) { }

        public async Task<IEnumerable<Order>> GetOrdersByUserName(string userName)
        {
            return await _context
                    .Orders
                    .Where(x => x.UserName == userName)
                    .ToListAsync();
        }
    }
}
