using Microsoft.EntityFrameworkCore;
using Order.Core.Entities;
using Order.Core.Repositories;
using Order.Infrastructure.Data;

namespace Order.Infrastructure.Repositories
{
    public class OrderRepository : RepositoryBase<OrderEntity>, IOrderRepository
    {
        private readonly OrderContext orderContext;

        public OrderRepository(OrderContext orderContext) : base(orderContext)
        {
            this.orderContext = orderContext;
        }
        public async Task<IEnumerable<OrderEntity>> GetOrdersByUserName(string userName)
        {
            return await orderContext.Orders
                .Where(x => x.UserName == userName)
                .ToListAsync();
        }
    }
}
