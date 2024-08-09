using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Papara.Business.Commands.ChangePassword;
using Papara.Business.Commands.CreateCategory;
using Papara.Business.Commands.Register;
using Papara.Business.DTOs.Authorization;
using Papara.Business.Queries.Login;
using Papara.Business.Queries.Logout;
using Papara.Business.Response;

namespace Papara.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorizationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthorizationController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<ResponseHandler<AuthorizationResponse>> Login([FromBody] AuthorizationRequest value)
        {
            var operation = new LoginQuery(value);
            var result = await _mediator.Send(operation);
            return result;
        }

        [HttpPost("Logout")]
        [Authorize]
        public async Task<ResponseHandler> Logout()
        {
            var operation = new LogoutQuery();
            var result = await _mediator.Send(operation);
            return result;
        }

        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<ResponseHandler> Register([FromBody] RegisterRequest value)
        {
            var operation = new RegisterCommand(value);
            var result = await _mediator.Send(operation);
            return result;
        }

        [HttpPost("ChangePassword")]
        [Authorize]
        public async Task<ResponseHandler> ChangePassword([FromBody] ChangePasswordRequest value)
        {
            var operation = new ChangePasswordCommand(value);
            var result = await _mediator.Send(operation);
            return result;
        }
    }
}
