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

        /// <summary>
        /// Authenticates the user based on the provided credentials.
        /// </summary>
        /// <param name="value">The authorization request containing the user's login credentials.</param>
        /// <returns>A response containing the authentication token and user details if successful.</returns>
        /// <remarks>
        /// This endpoint allows users to log in by providing their email and password. If the credentials are correct, 
        /// a JWT token is generated, which can be used for accessing secured endpoints.
        /// </remarks>
        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<ResponseHandler<AuthorizationResponse>> Login([FromBody] AuthorizationRequest value)
        {
            var operation = new LoginQuery(value);
            var result = await _mediator.Send(operation);
            return result;
        }

        /// <summary>
        /// Logs out the authenticated user.
        /// </summary>
        /// <returns>An empty response indicating that the logout was successful.</returns>
        /// <remarks>
        /// This endpoint terminates the user's session by invalidating the JWT token, preventing further access to secured endpoints.
        /// </remarks>
        [HttpPost("Logout")]
        [Authorize]
        public async Task<ResponseHandler> Logout()
        {
            var operation = new LogoutQuery();
            var result = await _mediator.Send(operation);
            return result;
        }

        /// <summary>
        /// Registers a new user with the provided details.
        /// </summary>
        /// <param name="value">The registration request containing the user's information.</param>
        /// <returns>A response indicating whether the registration was successful.</returns>
        /// <remarks>
        /// This endpoint allows new users to create an account by providing their email, password, and other required details.
        /// </remarks>
        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<ResponseHandler> Register([FromBody] RegisterRequest value)
        {
            var operation = new RegisterCommand(value);
            var result = await _mediator.Send(operation);
            return result;
        }

        /// <summary>
        /// Changes the password for the authenticated user.
        /// </summary>
        /// <param name="value">The change password request containing the current and new password.</param>
        /// <returns>A response indicating whether the password change was successful.</returns>
        /// <remarks>
        /// This endpoint allows authenticated users to change their password by providing their current password and the new password they wish to set.
        /// </remarks>
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
