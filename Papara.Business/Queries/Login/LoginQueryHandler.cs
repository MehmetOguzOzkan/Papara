using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Papara.Business.DTOs.Authorization;
using Papara.Business.DTOs.Product;
using Papara.Business.Response;
using Papara.Business.Session;
using Papara.Business.Token;
using Papara.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papara.Business.Queries.Login
{
    internal class LoginQueryHandler : IRequestHandler<LoginQuery, ResponseHandler<AuthorizationResponse>>
    {
        private readonly JwtConfig _jwtConfig;
        private readonly ITokenService _tokenService;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IValidator<AuthorizationRequest> _validator;

        public LoginQueryHandler(JwtConfig jwtConfig, ITokenService tokenService, UserManager<User> userManager, SignInManager<User> signInManager, IValidator<AuthorizationRequest> validator)
        {
            _jwtConfig = jwtConfig;
            _tokenService = tokenService;
            _userManager = userManager;
            _signInManager = signInManager;
            _validator = validator;
        }

        public async Task<ResponseHandler<AuthorizationResponse>> Handle(LoginQuery request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request.Request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return new ResponseHandler<AuthorizationResponse>(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
            }

            var loginResult = await _signInManager.PasswordSignInAsync(request.Request.UserName, request.Request.Password, true, false);
            if (!loginResult.Succeeded)
                return new ResponseHandler<AuthorizationResponse>("Login failed.");

            var user = await _userManager.FindByNameAsync(request.Request.UserName);
            if (user == null)
                return new ResponseHandler<AuthorizationResponse>("Login failed.");

            var token = await _tokenService.GenerateToken(user);
            var response = new AuthorizationResponse
            {
                AccessToken = token,
                UserName = user.UserName,
                ExpireTime = DateTime.Now.AddMinutes(_jwtConfig.AccessTokenExpiration)
            };

            return new ResponseHandler<AuthorizationResponse>(response);
        }
    }
}
