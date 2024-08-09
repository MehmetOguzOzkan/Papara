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

namespace Papara.Business.Commands.CreateUserAsAdmin
{
    internal class CreateUserAsAdminCommandHandler : IRequestHandler<CreateUserAsAdminCommand, ResponseHandler>
    {
        private readonly UserManager<User> _userManager;
        private readonly IValidator<RegisterRequest> _validator;

        public CreateUserAsAdminCommandHandler(UserManager<User> userManager, IValidator<RegisterRequest> validator)
        {
            _userManager = userManager;
            _validator = validator;
        }

        public async Task<ResponseHandler> Handle(CreateUserAsAdminCommand request, CancellationToken cancellationToken)
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
                return new ResponseHandler("Register failed.");
            }

            var addRoleResponse = await _userManager.AddToRoleAsync(newUser, "Admin");
            if (!addRoleResponse.Succeeded)
            {
                return new ResponseHandler("Register failed.");
            }

            return new ResponseHandler("Register successfully.");
        }
    }
}
