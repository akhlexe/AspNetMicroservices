using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ordering.Application.Features.Orders.Commands.CheckoutOrder;
using Ordering.Application.Features.Orders.Commands.UpdateOrder;
using Ordering.Application.Features.Orders.Queries.GetOrdersList;
using System.Net;

namespace Ordering.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrderController(IMediator mediator) => _mediator = mediator;

        [HttpGet("{userName}", Name = "GetOrder")]
        [ProducesResponseType(typeof(OrderVm), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<List<OrderVm>>> GetOrdersByUsername(string userName)
        {
            var query = new GetOrdersListQuery { UserName = userName };
            List<OrderVm> result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPost(Name = "CheckoutOrder")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult<int>> CheckoutOrder([FromBody] CheckoutOrderCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPut(Name = "UpdateOrder")]
        [ProducesDefaultResponseType]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult<bool>> UpdateOrder([FromBody] UpdateOrderCommand command)
        {
            bool result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
