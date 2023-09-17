namespace Basket.Core.Entities
{
    public class ShoppingCart
    {
        public string UserName { get; }
        public List<ShoppingCartItem> Items { get; private set; }

        public ShoppingCart(string userName, List<ShoppingCartItem> items)
        {
            UserName = userName;
            Items = items;
        }

        public ShoppingCart ValidateItems()
        {
            if(Items.Count == 0) throw new ArgumentException("Shopping card is empty");
            return this;
        }
    }
}
