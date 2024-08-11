using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Papara.Business.Commands.Payment;
using Papara.Business.Commands.PaymentWithoutCard;
using Papara.Business.DTOs.Order;
using Papara.Business.DTOs.Payment;
using Papara.Business.Response;

namespace Papara.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PaymentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Processes a payment using a credit or debit card.
        /// </summary>
        /// <remarks>
        /// This endpoint allows users to make a payment using a card. The user needs to provide payment details in the request body.
        /// </remarks>
        /// <param name="value">The payment details.</param>
        /// <response code="200">Returns the payment processing result.</response>
        /// <response code="400">Invalid payment details.</response>
        /// <response code="401">Unauthorized access.</response>
        [HttpPost("Card")]
        [Authorize(Roles = "user,admin")]
        public async Task<ResponseHandler<PaymentResponse>> PostWithCard([FromBody] PaymentRequest value)
        {
            var operation = new PaymentCommand(value);
            var result = await _mediator.Send(operation);
            return result;
        }

        /// <summary>
        /// Processes a payment using wallet balance without a card.
        /// </summary>
        /// <remarks>
        /// This endpoint allows users to make a payment using their wallet balance. The user needs to provide payment details in the request body.
        /// </remarks>
        /// <param name="value">The payment details without a card.</param>
        /// <response code="200">Returns the order processing result.</response>
        /// <response code="400">Invalid payment details.</response>
        /// <response code="401">Unauthorized access.</response>
        [HttpPost("Wallet")]
        [Authorize(Roles = "user,admin")]
        public async Task<ResponseHandler<OrderResponse>> PostWithoutCard([FromBody] PaymentRequestWithoutCard value)
        {
            var operation = new PaymentWithoutCardCommand(value);
            var result = await _mediator.Send(operation);
            return result;
        }
    }
}
