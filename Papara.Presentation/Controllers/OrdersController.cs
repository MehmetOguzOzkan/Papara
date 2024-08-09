using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Papara.Business.Commands.CreateOrder;
using Papara.Business.Commands.DeleteOrder;
using Papara.Business.DTOs.Order;
using Papara.Business.Queries.GetAllOrder;
using Papara.Business.Queries.GetAllOrderByUser;
using Papara.Business.Queries.GetOrderByCode;
using Papara.Business.Queries.GetOrderById;
using Papara.Business.Response;

namespace Papara.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrdersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<ResponseHandler<IEnumerable<OrderResponse>>> GetAll()
        {
            var operation = new GetAllOrderQuery();
            var result = await _mediator.Send(operation);
            return result;
        }

        [HttpGet("Users/{userId}")]
        [Authorize(Roles = "user,admin")]
        public async Task<ResponseHandler<IEnumerable<OrderResponse>>> GetAllByUser(Guid userId)
        {
            var operation = new GetAllOrderByUserQuery(userId);
            var result = await _mediator.Send(operation);
            return result;
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "user,admin")]
        public async Task<ResponseHandler<OrderResponse>> Get(Guid id)
        {
            var operation = new GetOrderByIdQuery(id);
            var result = await _mediator.Send(operation);
            return result;
        }

        [HttpGet("Codes/{code}")]
        [Authorize(Roles = "user,admin")]
        public async Task<ResponseHandler<OrderResponse>> GetByCode(int code)
        {
            var operation = new GetOrderByCodeQuery(code);
            var result = await _mediator.Send(operation);
            return result;
        }

        [HttpPost]
        [Authorize(Roles = "user,admin")]
        public async Task<ResponseHandler<OrderResponse>> Post([FromBody] OrderRequest value)
        {
            var operation = new CreateOrderCommand(value);
            var result = await _mediator.Send(operation);
            return result;
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "user,admin")]
        public async Task<ResponseHandler> Delete(Guid id)
        {
            var operation = new DeleteOrderCommand(id);
            var result = await _mediator.Send(operation);
            return result;
        }
    }
}
