using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Papara.Business.Commands.CreateUserAsAdmin;
using Papara.Business.Commands.UpdateUser;
using Papara.Business.DTOs.Authorization;
using Papara.Business.DTOs.User;
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

        [HttpPost("AsAdmin")]
        [Authorize(Roles = "admin")]
        public async Task<ResponseHandler> CreateUserAsAdmin([FromBody] RegisterRequest value)
        {
            var operation = new CreateUserAsAdminCommand(value);
            var result = await _mediator.Send(operation);
            return result;
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "user,admin")]
        public async Task<ResponseHandler> Put(Guid id, [FromBody] UserRequest value)
        {
            var operation = new UpdateUserCommand(id, value);
            var result = await _mediator.Send(operation);
            return result;
        }
    }
}
