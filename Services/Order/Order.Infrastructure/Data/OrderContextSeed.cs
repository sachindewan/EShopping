using Microsoft.Extensions.Logging;
using Order.Core.Entities;
using Order.Infrastructure.Data;

namespace Ordering.Infrastructure.Data;

public class OrderContextSeed
{
    public static async Task SeedAsync(OrderContext orderContext, ILogger<OrderContextSeed> logger)
    {
        if (!orderContext.Orders.Any())
        {
            orderContext.Orders.AddRange(GetOrders());
            await orderContext.SaveChangesAsync();
            logger.LogInformation($"Ordering Database: {typeof(OrderContext).Name} seeded.");
        }
    }

    private static IEnumerable<OrderEntity> GetOrders()
    {
        return new List<OrderEntity>
        {
            new()
            {
                UserName = "sachin",
                FirstName = "sachin",
                LastName = "kumar",
                EmailAddress = "sachinkumar@eshop.net",
                AddressLine = "Bangalore",
                Country = "India",
                TotalPrice = 750,
                State = "KA",
                ZipCode = "560001",

                CardName = "Visa",
                CardNumber = "1234567890123456",
                CreatedBy = "Rahul",
                Expiration = "12/25",
                Cvv = "123",
                PaymentMethod = 1,
                LastModifiedBy = "Rahul",
                LastModifiedDate = new DateTime(),
            }
        };
    }
}