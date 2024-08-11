using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Papara.Business.Commands.CreateUserAsAdmin;
using Papara.Business.Commands.UpdateUser;
using Papara.Business.Commands.UpdateWalletBalance;
using Papara.Business.DTOs.Authorization;
using Papara.Business.DTOs.User;
using Papara.Business.Queries.GetLoyaltyPointByUser;
using Papara.Business.Queries.GetWalletBalanceByUser;
using Papara.Business.Response;

namespace Papara.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Retrieves the loyalty points balance of the current user.
        /// </summary>
        /// <remarks>
        /// This endpoint allows users to get their current loyalty points balance.
        /// </remarks>
        /// <response code="200">Returns the loyalty points balance.</response>
        /// <response code="401">Unauthorized access.</response>
        [HttpGet("LoyaltyPoints")]
        [Authorize(Roles = "user,admin")]
        public async Task<ResponseHandler<object>> GetLoyaltyPoint()
        {
            var operation = new GetLoyaltyPointByUserQuery();
            var result = await _mediator.Send(operation);
            return result;
        }

        /// <summary>
        /// Retrieves the wallet balance of the current user.
        /// </summary>
        /// <remarks>
        /// This endpoint allows users to get their current wallet balance.
        /// </remarks>
        /// <response code="200">Returns the wallet balance.</response>
        /// <response code="401">Unauthorized access.</response>
        [HttpGet("WalletBalance")]
        [Authorize(Roles = "user,admin")]
        public async Task<ResponseHandler<object>> GetWalletBalance()
        {
            var operation = new GetWalletBalanceByUserQuery();
            var result = await _mediator.Send(operation);
            return result;
        }

        /// <summary>
        /// Creates a new user as an administrator.
        /// </summary>
        /// <remarks>
        /// This endpoint allows administrators to create a new user account.
        /// </remarks>
        /// <param name="value">The user data to be created.</param>
        /// <response code="201">Returns the created user details.</response>
        /// <response code="400">Invalid user data.</response>
        [HttpPost("AsAdmin")]
        [Authorize(Roles = "admin")]
        public async Task<ResponseHandler> CreateUserAsAdmin([FromBody] RegisterRequest value)
        {
            var operation = new CreateUserAsAdminCommand(value);
            var result = await _mediator.Send(operation);
            return result;
        }

        /// <summary>
        /// Updates the details of an existing user.
        /// </summary>
        /// <remarks>
        /// This endpoint allows users or administrators to update user details.
        /// </remarks>
        /// <param name="id">The unique identifier of the user to be updated.</param>
        /// <param name="value">The updated user data.</param>
        /// <response code="200">Returns the updated user details.</response>
        /// <response code="400">Invalid user data.</response>
        /// <response code="404">User not found.</response>
        [HttpPut("{id}")]
        [Authorize(Roles = "user,admin")]
        public async Task<ResponseHandler> Put(Guid id, [FromBody] UserRequest value)
        {
            var operation = new UpdateUserCommand(id, value);
            var result = await _mediator.Send(operation);
            return result;
        }

        /// <summary>
        /// Updates the wallet balance of the current user.
        /// </summary>
        /// <remarks>
        /// This endpoint allows users or administrators to update the wallet balance.
        /// </remarks>
        /// <param name="value">The data for updating the wallet balance.</param>
        /// <response code="200">Returns the result of the wallet balance update.</response>
        /// <response code="400">Invalid balance data.</response>
        /// <response code="401">Unauthorized access.</response>
        [HttpPatch("WalletBalance")]
        [Authorize(Roles = "user,admin")]
        public async Task<ResponseHandler> UpdateWalletBalance([FromBody] MoneyTransferRequest value)
        {
            var operation = new UpdateWalletBalanceCommand(value);
            var result = await _mediator.Send(operation);
            return result;
        }
    }
}
