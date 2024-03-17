using Microsoft.EntityFrameworkCore;
using Order.Core.Common;
using Order.Core.Entities;

namespace Order.Infrastructure.Data
{
    public class OrderContext :DbContext
    {
        public OrderContext(DbContextOptions<OrderContext> options) : base(options)
        {
            
        }
        public DbSet<OrderEntity> Orders { get; set; }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<EntityBase>())
            {
                switch(entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedDate = DateTime.Now.ToString();
                        entry.Entity.CreatedBy = "sachin"; //TODO: This will be replaced Identity Server
                        break;
                    case EntityState.Modified:
                        entry.Entity.LastModifiedDate = DateTime.Now.ToString();
                        entry.Entity.LastModifiedBy = "sachin"; //TODO: This will be replaced Identity Server
                        break;
                }
            }
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
