﻿using Microsoft.Extensions.Logging;
using Ordering.Domain.Entities;

namespace Ordering.Infrastructure.Persistence
{
    public class OrderContextSeed
    {
        public static async Task SeedAsync(OrderContext orderContext, ILogger<OrderContextSeed> logger)
        {
            if (!orderContext.Orders.Any())
            {
                orderContext.Orders.AddRange(GetPreconfiguredOrders());

                await orderContext.SaveChangesAsync();

                logger.LogInformation("Seed database associated with context {DbContextName}", typeof(OrderContext).Name);
            }
        }

        public static IEnumerable<Order> GetPreconfiguredOrders()
        {
            return new List<Order>()
            {
                new Order()
                {
                    UserName = "exequiel",
                    FirstName = "Exe",
                    LastName = "Piñero",
                    EmailAddress = "exepine@gmail.com",
                    AddressLine = "AddressOne",
                    Country = "Argentina",
                    TotalPrice = 350
                }
            };
        }
    }
}
