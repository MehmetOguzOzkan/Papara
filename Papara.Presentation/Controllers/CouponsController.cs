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

        /// <summary>
        /// Retrieves all coupons.
        /// </summary>
        /// <remarks>
        /// This endpoint is accessible only by users with the "admin" role. It returns a list of all available coupons in the system.
        /// </remarks>
        /// <response code="200">Returns a list of all coupons.</response>
        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<ResponseHandler<IEnumerable<CouponResponse>>> GetAllCoupons()
        {
            var query = new GetAllCouponsQuery();
            var result = await _mediator.Send(query);
            return result;
        }

        /// <summary>
        /// Retrieves a coupon by its unique identifier.
        /// </summary>
        /// <remarks>
        /// This endpoint is accessible by both "user" and "admin" roles. It returns the details of the coupon identified by the given ID.
        /// </remarks>
        /// <param name="id">The unique identifier of the coupon.</param>
        /// <response code="200">Returns the coupon details.</response>
        /// <response code="404">Coupon not found.</response>
        [HttpGet("{id:guid}")]
        [Authorize(Roles = "user,admin")]
        public async Task<ResponseHandler<CouponResponse>> GetCouponById(Guid id)
        {
            var query = new GetCouponByIdQuery(id);
            var result = await _mediator.Send(query);
            return result;
        }

        /// <summary>
        /// Retrieves all coupons available for the current user.
        /// </summary>
        /// <remarks>
        /// This endpoint is accessible by both "user" and "admin" roles. It returns a list of coupons associated with the currently authenticated user.
        /// </remarks>
        /// <response code="200">Returns a list of coupons for the current user.</response>
        /// <response code="404">No coupons found for the current user.</response>
        [HttpGet("Users/Coupons")]
        [Authorize(Roles = "user,admin")]
        public async Task<ResponseHandler<IEnumerable<CouponResponse>>> GetCouponsByUser()
        {
            var query = new GetAllCouponByUserQuery();
            var result = await _mediator.Send(query);
            return result;
        }

        /// <summary>
        /// Retrieves a coupon by its unique code.
        /// </summary>
        /// <remarks>
        /// This endpoint is accessible by both "user" and "admin" roles. It returns the coupon details identified by the provided code.
        /// </remarks>
        /// <param name="code">The unique code of the coupon.</param>
        /// <response code="200">Returns the coupon details.</response>
        /// <response code="404">Coupon not found.</response>
        [HttpGet("Codes/{code}")]
        [Authorize(Roles = "user,admin")]
        public async Task<ResponseHandler<CouponResponse>> GetCouponByCode(string code)
        {
            var query = new GetCouponByCodeQuery(code);
            var result = await _mediator.Send(query);
            return result;
        }

        /// <summary>
        /// Creates a new coupon.
        /// </summary>
        /// <remarks>
        /// This endpoint is accessible only by users with the "admin" role. It creates a new coupon using the provided request data.
        /// </remarks>
        /// <param name="value">The coupon data to be created.</param>
        /// <response code="201">Returns the created coupon details.</response>
        /// <response code="400">Invalid request data.</response>
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ResponseHandler<CouponResponse>> CreateCoupon([FromBody] CouponRequest value)
        {
            var command = new CreateCouponCommand(value);
            var result = await _mediator.Send(command);
            return result;
        }

        /// <summary>
        /// Updates an existing coupon.
        /// </summary>
        /// <remarks>
        /// This endpoint is accessible only by users with the "admin" role. It updates the coupon identified by the provided ID using the provided request data.
        /// </remarks>
        /// <param name="id">The unique identifier of the coupon to be updated.</param>
        /// <param name="value">The updated coupon data.</param>
        /// <response code="200">Returns the updated coupon details.</response>
        /// <response code="400">Invalid request data.</response>
        /// <response code="404">Coupon not found.</response>
        [HttpPut("{id:guid}")]
        [Authorize(Roles = "admin")]
        public async Task<ResponseHandler<CouponResponse>> UpdateCoupon(Guid id, [FromBody] CouponRequest value)
        {
            var command = new UpdateCouponCommand(id, value);
            var result = await _mediator.Send(command);
            return result;
        }

        /// <summary>
        /// Deletes a coupon.
        /// </summary>
        /// <remarks>
        /// This endpoint is accessible only by users with the "admin" role. It deletes the coupon identified by the provided ID.
        /// </remarks>
        /// <param name="id">The unique identifier of the coupon to be deleted.</param>
        /// <response code="204">Coupon successfully deleted.</response>
        /// <response code="404">Coupon not found.</response>
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
