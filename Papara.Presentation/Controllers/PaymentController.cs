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

        [HttpPost("Card")]
        [Authorize(Roles = "user,admin")]
        public async Task<ResponseHandler<PaymentResponse>> PostWithCard([FromBody] PaymentRequest value)
        {
            var operation = new PaymentCommand(value);
            var result = await _mediator.Send(operation);
            return result;
        }

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
