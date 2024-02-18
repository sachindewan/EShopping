using Catalog.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Catalog.Infrastructure.Data
{
    public class CatalogContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductType> Types { get; set; }
        public DbSet<ProductBrand> Brands { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public CatalogContext(DbContextOptions<CatalogContext> options):base(options)
        {
            
        }

    }
    public static class Extensions
    {
        public static void CreateDbIfNotExists(this IHost host)
        {
            using var scope = host.Services.CreateScope();

            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<CatalogContext>();
            if(context.Database.GetPendingMigrations().Count() > 0)
            {
                context.Database.Migrate();
            }
            context.Database.EnsureCreated();
            DbInitializer.Initialize(context);
        }
    }


    public static class DbInitializer
    {
        public static void Initialize(CatalogContext context)
        {
            if (context.Products.Any())
                return;

            var products = new List<Product>
        {
            new Product { Id="1", Name = "Solar Powered Flashlight", Description = "A fantastic product for outdoor enthusiasts", Price = 19.99m, ImageUrl = "product1.png"
                          , Brands = new ProductBrand { Id="1", Name ="Temp" } , Types = new ProductType {Id="1", Name = "Power" } },
            new Product {Id="2", Name = "Hiking Poles", Description = "Ideal for camping and hiking trips", Price = 24.99m, ImageUrl = "product2.png",Brands = new ProductBrand {Id="2", Name ="Temp" } , Types = new ProductType {Id = "2",  Name = "Power" } },
            new Product {Id="3",Name = "Outdoor Rain Jacket", Description = "This product will keep you warm and dry in all weathers", Price = 49.99m, ImageUrl = "product3.png", Brands = new ProductBrand { Id = "3", Name = "Temp" }, Types = new ProductType { Id = "3", Name = "Power" }},
            new Product {Id="4",Name = "Survival Kit", Description = "A must-have for any outdoor adventurer", Price = 99.99m, ImageUrl = "product4.png", Brands = new ProductBrand { Id = "4", Name = "Temp" }, Types = new ProductType { Id = "4", Name = "Power" }},
            new Product {Id="5",Name = "Outdoor Backpack", Description = "This backpack is perfect for carrying all your outdoor essentials", Price = 39.99m, ImageUrl = "product5.png", Brands = new ProductBrand { Id = "5", Name = "Temp" }, Types = new ProductType { Id = "5", Name = "Power" }},
            new Product {Id="6",Name = "Camping Cookware", Description = "This cookware set is ideal for cooking outdoors", Price = 29.99m, ImageUrl = "product6.png", Brands = new ProductBrand { Id = "6", Name = "Temp" }, Types = new ProductType { Id = "6", Name = "Power" }},
            new Product {Id="7",Name = "Camping Stove", Description = "This stove is perfect for cooking outdoors", Price = 49.99m, ImageUrl = "product7.png", Brands = new ProductBrand { Id = "7", Name = "Temp" }, Types = new ProductType { Id = "7", Name = "Power" }},
            new Product {Id="8",Name = "Camping Lantern", Description = "This lantern is perfect for lighting up your campsite", Price = 19.99m, ImageUrl = "product8.png", Brands = new ProductBrand { Id = "8", Name = "Temp" }, Types = new ProductType { Id = "8", Name = "Power" }},
            new Product {Id="9",Name = "Camping Tent", Description = "This tent is perfect for camping trips", Price = 99.99m, ImageUrl = "product9.png", Brands = new ProductBrand { Id = "9", Name = "Temp" }, Types = new ProductType { Id = "9", Name = "Power" }},
            new Product {Id="10",Name = "Camping Tent 2", Description = "This updated tent is improved and cheaper, perfrect for your next trip.", Price = 79.99m, ImageUrl = "product9.png", Brands = new ProductBrand { Id = "10", Name = "Temp" }, Types = new ProductType { Id = "10", Name = "Power" }},
        };

            context.AddRange(products);

            context.SaveChanges();
        }
    }

}
