using Catalog.Core.Entities;
using MongoDB.Driver;
using System.Text.Json;

namespace Catalog.Infrastructure.Data
{
    public static class CatalogContextSeed
    {
        public static void SeedData(IMongoCollection<Product> productCollection)
        {
            var checkBrands = productCollection.Find(_ => true).Any();
            var path = Path.Combine("Data", "SeedData", "products.json");
            if (checkBrands is false)
            {
                var brandsData = File.ReadAllText(path);
                var brands = JsonSerializer.Deserialize<List<Product>>(brandsData);
                if (brands != null)
                {
                    foreach (var item in brands)
                    {
                        productCollection.InsertOneAsync(item);
                    }
                }
            }
        }
    }
}
