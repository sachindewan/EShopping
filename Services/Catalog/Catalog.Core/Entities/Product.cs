
namespace Catalog.Core.Entities
{
    public class Product :BaseEntity
    {
        public string Name { get; set; }
        public string Summary { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public ProductBrand Brands { get; set; }
        public ProductType Types { get; set; }
        public decimal Price { get; set; }
    }
}
