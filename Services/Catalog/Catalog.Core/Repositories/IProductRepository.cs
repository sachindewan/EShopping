using Catalog.Core.Entities;
using Catalog.Core.Specs;
using System.Linq.Expressions;

namespace Catalog.Core.Repositories
{
    public interface IProductRepository
    {
        public Task<Pagination<Product>> GetAsync(CatalogSpecParams catalogSpecParams, Expression<Func<Product, bool>> filter = null, Func<IQueryable<Product>, IOrderedQueryable<Product>> orderBy = null, string includeProperties = "");
        Task<Product> GetProduct(string id);
        Task<IEnumerable<Product>> GetProductByName(string name);
        Task<IEnumerable<Product>> GetProductByBrand(string name);
        Task<Product> CreateProduct(Product product);
        Task<bool> UpdateProduct(Product product);
        Task<bool> DeleteProduct(string id);
    }
}
