﻿using Microsoft.AspNetCore.Mvc;
using Shopping.Aggregator.Models;
using Shopping.Aggregator.Services;
using System.Net;

namespace Shopping.Aggregator.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ShoppingController : ControllerBase
    {
        private readonly ICatalogService _catalogService;
        private readonly IBasketService _basketService;
        private readonly IOrderService _orderService;

        public ShoppingController(
            ICatalogService catalogService,
            IBasketService basketService,
            IOrderService orderService)
        {
            _catalogService = catalogService;
            _basketService = basketService;
            _orderService = orderService;
        }

        [HttpGet("{userName}", Name = "GetShopping")]
        [ProducesResponseType(typeof(ShoppingModel), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ShoppingModel>> GetShopping(string userName)
        {
            BasketModel basket = await _basketService.GetBasket(userName);

            foreach (var item in basket.Items)
            {
                var product = await _catalogService.GetCatalog(item.ProductId);

                // Set additional fields

                item.ProductName = product.Name;
                item.Summary = product.Summary;
                item.ImageFile = product.ImageFile;
                item.Description = product.Description;
                item.Category = product.Category;
            }

            IEnumerable<OrderResponseModel> orders = await _orderService.GetOrdersByUserName(userName);

            return Ok(new ShoppingModel
            {
                UserName = userName,
                BasketWithProducts = basket,
                Orders = orders
            });
        }
    }
}
