using AutoMapper;
using Basket.API.Entities;
using Basket.API.GrpcServices;
using Basket.API.Repositories;
using EventBus.Messages.Events;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Basket.API.Controllers
{
    [ApiController]
    [Route("/api/v1/[controller]")]
    public class BasketController : ControllerBase
    {
        private readonly IBasketRepository _repository;
        private readonly DiscountGrpcService _discountGrpcService;
        private readonly IMapper _mapper;
        private readonly IPublishEndpoint _publishEndpoint;

        public BasketController(
            IBasketRepository repository,
            DiscountGrpcService discountGrpcService,
            IMapper mapper,
            IPublishEndpoint publishEndpoint)
        {
            _repository = repository ?? throw new ArgumentException(nameof(repository));
            _discountGrpcService = discountGrpcService ?? throw new ArgumentException(nameof(discountGrpcService));
            _mapper = mapper ?? throw new ArgumentException(nameof(mapper));
            _publishEndpoint = publishEndpoint ?? throw new ArgumentException(nameof(publishEndpoint));
        }

        [HttpGet("{userName}", Name = "GetBasket")]
        [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ShoppingCart>> GetBasket(string userName)
        {
            ShoppingCart Basket = await _repository.GetBasket(userName);
            return Ok(Basket ?? new ShoppingCart(userName));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ShoppingCart>> UpdateBasket([FromBody] ShoppingCart basket)
        {
            foreach (var item in basket.Items)
            {
                var coupon = await _discountGrpcService.GetDiscount(item.ProductName);
                item.Price -= coupon.Amount;
            }

            return Ok(await _repository.UpdateBasket(basket));
        }

        [HttpDelete("{userName}", Name = "DeleteBasket")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> DeleteBasket(string userName)
        {
            await _repository.DeleteBasket(userName);
            return Ok();
        }
        [Route("[action]")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Checkout([FromBody] BasketCheckout basketCheckout)
        {
            // Get basket with total price
            ShoppingCart basket = await _repository.GetBasket(basketCheckout.UserName);

            if(basket == null)
            {
                return BadRequest();
            }

            // Create checkout event
            BasketCheckoutEvent basketCheckoutEvent = _mapper.Map<BasketCheckoutEvent>(basketCheckout);
            basketCheckoutEvent.TotalPrice = basket.TotalPrice;

            // Send checkout event
            await _publishEndpoint.Publish(basketCheckoutEvent);

            // Remove the basket
            await _repository.DeleteBasket(basket.UserName);

            return Accepted();
        }
    }
}
