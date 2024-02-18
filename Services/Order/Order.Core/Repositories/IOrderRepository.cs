using Order.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order.Core.Repositories
{
    public interface IOrderRepository: IAsyncRepository<OrderEntity>
    {
        Task<IEnumerable<OrderEntity>> GetOrdersByUserName(string userName);
    }
}
