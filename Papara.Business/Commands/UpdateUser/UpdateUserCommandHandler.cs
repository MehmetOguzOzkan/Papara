using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Papara.Business.DTOs.User;
using Papara.Business.Response;
using Papara.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papara.Business.Commands.UpdateUser
{
    internal class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, ResponseHandler>
    {
        private readonly UserManager<User> _userManager;
        private readonly IValidator<UserRequest> _validator;

        public UpdateUserCommandHandler(UserManager<User> userManager, IValidator<UserRequest> validator)
        {
            _userManager = userManager;
            _validator = validator;
        }

        public async Task<ResponseHandler> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request.Request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return new ResponseHandler(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
            }

            var user = await _userManager.FindByIdAsync(request.Id.ToString());
            if (user == null)
                return new ResponseHandler("User not found.");

            user.FirstName = request.Request.FirstName;
            user.LastName = request.Request.LastName;
            user.Email = request.Request.Email;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return new ResponseHandler("Update failed.");

            return new ResponseHandler("User updated successfully.");
        }
    }
}
