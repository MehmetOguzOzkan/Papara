using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Papara.Business.Commands.CreateCoupon;
using Papara.Business.Commands.DeleteCoupon;
using Papara.Business.Commands.UpdateCoupon;
using Papara.Business.DTOs.Coupon;
using Papara.Business.Queries.GetAllCoupon;
using Papara.Business.Queries.GetAllCouponByUser;
using Papara.Business.Queries.GetCouponByCode;
using Papara.Business.Queries.GetCouponById;
using Papara.Business.Response;

namespace Papara.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CouponsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CouponsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<ResponseHandler<IEnumerable<CouponResponse>>> GetAllCoupons()
        {
            var query = new GetAllCouponsQuery();
            var result = await _mediator.Send(query);
            return result;
        }

        [HttpGet("{id:guid}")]
        [Authorize(Roles = "user,admin")]
        public async Task<ResponseHandler<CouponResponse>> GetCouponById(Guid id)
        {
            var query = new GetCouponByIdQuery(id);
            var result = await _mediator.Send(query);
            return result;
        }

        [HttpGet("Users/{userId:guid}")]
        [Authorize(Roles = "user,admin")]
        public async Task<ResponseHandler<IEnumerable<CouponResponse>>> GetCouponsByUser(Guid userId)
        {
            var query = new GetAllCouponByUserQuery(userId);
            var result = await _mediator.Send(query);
            return result;
        }

        [HttpGet("Codes/{code}")]
        [Authorize(Roles = "user,admin")]
        public async Task<ResponseHandler<CouponResponse>> GetCouponByCode(string code)
        {
            var query = new GetCouponByCodeQuery(code);
            var result = await _mediator.Send(query);
            return result;
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ResponseHandler<CouponResponse>> CreateCoupon([FromBody] CouponRequest value)
        {
            var command = new CreateCouponCommand(value);
            var result = await _mediator.Send(command);
            return result;
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "admin")]
        public async Task<ResponseHandler<CouponResponse>> UpdateCoupon(Guid id, [FromBody] CouponRequest value)
        {
            var command = new UpdateCouponCommand(id, value);
            var result = await _mediator.Send(command);
            return result;
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "admin")]
        public async Task<ResponseHandler> DeleteCoupon(Guid id)
        {
            var command = new DeleteCouponCommand(id);
            var result = await _mediator.Send(command);
            return result;
        }
    }
}
