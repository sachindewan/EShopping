using Catalog.Core.Entities;
using Catalog.Core.Repositories;
using Catalog.Core.Specs;
using Catalog.Infrastructure.Data;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Data.Common;
using System.Linq.Expressions;
using static Dapper.SqlMapper;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Catalog.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository, IBrandRepository, ITypesRepository
    {
        private readonly CatalogContext _context;
        private readonly NpgsqlDataSource npgsqlDataSource;

        public ProductRepository(CatalogContext context,NpgsqlDataSource npgsqlDataSource)
        {
            _context = context;
            this.npgsqlDataSource = npgsqlDataSource;
        }

        public async Task<Pagination<Product>> GetAsync(CatalogSpecParams catalogSpecParams, Expression<Func<Product, bool>> filter = null, Func<IQueryable<Product>, IOrderedQueryable<Product>> orderBy = null, string includeProperties = "")
        {
            IQueryable<Product> query = _context.Products;

            if (filter == null)
            {
                if(catalogSpecParams.TypeId != null)
                {
                    filter = filter=>filter.Types.Id.Equals(catalogSpecParams.TypeId);
                }
                includeProperties += nameof(Product.Types);
            }

            return new Pagination<Product>
            {
                PageSize = catalogSpecParams.PageSize,
                PageIndex = catalogSpecParams.PageIndex,
                Data = await _context
                   .Products
                   .Where(filter)
                   .Skip(catalogSpecParams.PageSize * (catalogSpecParams.PageIndex - 1))
                   .Take(catalogSpecParams.PageSize).Include(includeProperties)
                   .ToListAsync(),
                Count = await _context.Products.CountAsync()
            };
        }
        public async Task<Product> GetProduct(string id)
        {
            string sql = "SELECT *\r\nFROM public.\"Products\" p\r\nINNER JOIN public.\"Brands\" b ON p.\"BrandsId\" = b.\"Id\"\r\nINNER JOIN public.\"Types\" t ON p.\"TypesId\" = t.\"Id\"\r\nWHERE p.\"Id\" = @id";
            using var connection = npgsqlDataSource.CreateConnection();
            await connection.OpenAsync();

            //using var command = connection.CreateCommand();
            //command.CommandText = sql;

            //using var reader = await command.ExecuteReaderAsync();

            //var examples = new List<Product>();
            //while (await reader.ReadAsync())
            //{
            //    var example = new Product
            //    {
            //        Id = reader.GetString(0),
            //        Name = reader.GetString(1),
            //    };

            //    examples.Add(example);
            //}
            var examples = await connection.QueryAsync<Product, ProductBrand,ProductType, Product>(sql, (product
                , brand,type) =>
            {
                product.Brands = brand;
                product.Types = type;
                return product;
            },new { id}, splitOn: "id");

            return examples.FirstOrDefault();
           // return await _context.Products.Where(x=>x.Id==id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Product>> GetProductByName(string name)
        {
            return await _context
                .Products
                .Where(x=>x.Name==name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductByBrand(string name)
        {
            return await _context
                .Products
                .Where(x=>x.Brands.Name == name)
                .ToListAsync();
        }

        public async Task<Product> CreateProduct(Product product)
        {
            await _context.Products.AddAsync(product);
            return product;
        }

        public async Task<bool> UpdateProduct(Product product)
        {
            var updateResult = await _context
                .Products
                  .Where(model => model.Id == product.Id)
                .ExecuteUpdateAsync(setters => setters
                  .SetProperty(m => m.Id, product.Id)
                  .SetProperty(m => m.Name, product.Name)
                  .SetProperty(m => m.Description, product.Description)
                  .SetProperty(m => m.Price, product.Price)
                  .SetProperty(m => m.ImageUrl, product.ImageUrl)
                );
            return updateResult > 0;
        }

        public async Task<bool> DeleteProduct(string id)
        {
            var deleteResult = await _context
                    .Products
                    .Where(model => model.Id == id)
                .ExecuteDeleteAsync();
            return deleteResult > 0;
        }

        public async Task<IEnumerable<ProductBrand>> GetAllBrands()
        {
            return await _context
                .Brands
                .ToListAsync();
        }

        public async Task<IEnumerable<ProductType>> GetAllTypes()
        {
            return await _context
                .Types
                .ToListAsync();
        }
    }
}
