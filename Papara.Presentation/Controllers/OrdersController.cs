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

        /// <summary>
        /// Retrieves all orders.
        /// </summary>
        /// <remarks>
        /// This endpoint is accessible only by users with the "admin" role. It returns a list of all orders in the system.
        /// </remarks>
        /// <response code="200">Returns a list of all orders.</response>
        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<ResponseHandler<IEnumerable<OrderResponse>>> GetAll()
        {
            var operation = new GetAllOrderQuery();
            var result = await _mediator.Send(operation);
            return result;
        }

        /// <summary>
        /// Retrieves all orders placed by the current user.
        /// </summary>
        /// <remarks>
        /// This endpoint is accessible by both "user" and "admin" roles. It returns a list of orders associated with the currently authenticated user.
        /// </remarks>
        /// <response code="200">Returns a list of orders for the current user.</response>
        /// <response code="404">No orders found for the current user.</response>
        [HttpGet("Users/Orders")]
        [Authorize(Roles = "user,admin")]
        public async Task<ResponseHandler<IEnumerable<OrderResponse>>> GetAllByUser()
        {
            var operation = new GetAllOrderByUserQuery();
            var result = await _mediator.Send(operation);
            return result;
        }

        /// <summary>
        /// Retrieves an order by its unique identifier.
        /// </summary>
        /// <remarks>
        /// This endpoint is accessible by both "user" and "admin" roles. It returns the details of the order identified by the given ID.
        /// </remarks>
        /// <param name="id">The unique identifier of the order.</param>
        /// <response code="200">Returns the order details.</response>
        /// <response code="404">Order not found.</response>
        [HttpGet("{id}")]
        [Authorize(Roles = "user,admin")]
        public async Task<ResponseHandler<OrderResponse>> Get(Guid id)
        {
            var operation = new GetOrderByIdQuery(id);
            var result = await _mediator.Send(operation);
            return result;
        }

        /// <summary>
        /// Retrieves an order by its unique code.
        /// </summary>
        /// <remarks>
        /// This endpoint is accessible by both "user" and "admin" roles. It returns the details of the order identified by the provided code.
        /// </remarks>
        /// <param name="code">The unique code of the order.</param>
        /// <response code="200">Returns the order details.</response>
        /// <response code="404">Order not found.</response>
        [HttpGet("Codes/{code}")]
        [Authorize(Roles = "user,admin")]
        public async Task<ResponseHandler<OrderResponse>> GetByCode(int code)
        {
            var operation = new GetOrderByCodeQuery(code);
            var result = await _mediator.Send(operation);
            return result;
        }

        /// <summary>
        /// Creates a new order.
        /// </summary>
        /// <remarks>
        /// This endpoint is accessible by users with the "user" or "admin" role. It creates a new order using the provided request data.
        /// </remarks>
        /// <param name="value">The order data to be created.</param>
        /// <response code="201">Returns the created order details.</response>
        /// <response code="400">Invalid request data.</response>
        [HttpPost]
        [Authorize(Roles = "user,admin")]
        public async Task<ResponseHandler<OrderResponse>> Create([FromBody] OrderRequest value)
        {
            var command = new CreateOrderCommand(value);
            var result = await _mediator.Send(command);
            return result;
        }

        /// <summary>
        /// Deletes an order.
        /// </summary>
        /// <remarks>
        /// This endpoint is accessible only by users with the "admin" role. It deletes the order identified by the provided ID.
        /// </remarks>
        /// <param name="id">The unique identifier of the order to be deleted.</param>
        /// <response code="204">Order successfully deleted.</response>
        /// <response code="404">Order not found.</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ResponseHandler> Delete(Guid id)
        {
            var command = new DeleteOrderCommand(id);
            var result = await _mediator.Send(command);
            return result;
        }
    }
}
