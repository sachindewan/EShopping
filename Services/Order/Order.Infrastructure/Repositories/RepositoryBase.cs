using Microsoft.EntityFrameworkCore;
using Order.Core.Common;
using Order.Core.Repositories;
using Order.Infrastructure.Data;

namespace Order.Infrastructure.Repositories
{
    public class RepositoryBase<T> : IAsyncRepository<T> where T : EntityBase
    {
        private readonly OrderContext orderContext;

        public RepositoryBase(OrderContext orderContext)
        {
            this.orderContext = orderContext;
        }
        public async Task<T> AddAsync(T entity)
        {
            await orderContext.Set<T>().AddAsync(entity);
            await orderContext.SaveChangesAsync();
            return entity;
        }

        public async Task DeleteAsync(T entity)
        {
            await orderContext.Set<T>().Where(x=>x.Id==entity.Id).ExecuteDeleteAsync();
            await orderContext.SaveChangesAsync();
        }

        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            return await orderContext.Set<T>().ToListAsync();
        }

        public async Task<IReadOnlyList<T>> GetAllAsync(System.Linq.Expressions.Expression<Func<T, bool>> predicate)
        {
            return await orderContext.Set<T>().Where(predicate).ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id) => await orderContext.Set<T>().FindAsync(id);

        public async Task UpdateAsync(T entity)
        {
            orderContext.Set<T>().Update(entity);
            //orderContext.Set<T>().Entry(entity).State = EntityState.Modified;
            await orderContext.SaveChangesAsync();
        }
    }
}
