using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Papara.Business.DTOs.Authorization;
using Papara.Business.Response;
using Papara.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papara.Business.Commands.Register
{
    internal class RegisterCommandHandler : IRequestHandler<RegisterCommand, ResponseHandler>
    {
        private readonly UserManager<User> _userManager;
        private readonly IValidator<RegisterRequest> _validator;

        public RegisterCommandHandler(UserManager<User> userManager, IValidator<RegisterRequest> validator)
        {
            _userManager = userManager;
            _validator = validator;
        }

        public async Task<ResponseHandler> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request.Request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return new ResponseHandler(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
            }

            var newUser = new User
            {
                UserName = request.Request.Email,
                FirstName = request.Request.FirstName,
                LastName = request.Request.LastName,
                Email = request.Request.Email,
                EmailConfirmed = true,
                TwoFactorEnabled = false,
            };

            var newUserResponse = await _userManager.CreateAsync(newUser, request.Request.Password);
            if (!newUserResponse.Succeeded)
            {
                return new ResponseHandler("The user was not successfully created as admin.");
            }

            var addRoleResponse = await _userManager.AddToRoleAsync(newUser, "User");
            if (!addRoleResponse.Succeeded)
            {
                return new ResponseHandler("The user was not successfully created as admin.");
            }

            return new ResponseHandler("The user has been successfully created as admin.");
        }
    }
}
