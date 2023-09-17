using Catalog.Core.Entities;
using MongoDB.Driver;
using System.Text.Json;

namespace Catalog.Infrastructure.Data
{
    public static class TypeContextSeed
    {
        public static void SeedData(IMongoCollection<ProductType> typeCollection)
        {
            var checkBrands = typeCollection.Find(_ => true).Any();
            var path = Path.Combine("Data", "SeedData", "types.json");
            if (checkBrands is false)
            {
                var brandsData = File.ReadAllText(path);
                var brands = JsonSerializer.Deserialize<List<ProductType>>(brandsData);
                if (brands != null)
                {
                    foreach (var item in brands)
                    {
                        typeCollection.InsertOneAsync(item);
                    }
                }
            }
        }
    }
}
