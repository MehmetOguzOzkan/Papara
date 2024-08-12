using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Papara.Business.DTOs.Authorization;
using Papara.Business.DTOs.Payment;
using Papara.Business.Response;
using Papara.Business.Session;
using Papara.Business.Validation;
using Papara.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papara.Business.Commands.ChangePassword
{
    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, ResponseHandler>
    {
        private readonly UserManager<User> _userManager;
        private readonly ISessionContext _sessionContext;
        private readonly IValidator<ChangePasswordRequest> _validator;

        public ChangePasswordCommandHandler(UserManager<User> userManager, ISessionContext sessionContext, IValidator<ChangePasswordRequest> validator)
        {
            _userManager = userManager;
            _sessionContext = sessionContext;
            _validator = validator;
        }

        public async Task<ResponseHandler> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request.Request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return new ResponseHandler(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
            }

            User user = await _userManager.GetUserAsync(_sessionContext.HttpContext.User);
            if (user == null)
                return new ResponseHandler("Login failed.");

            var userResponse = await _userManager.FindByEmailAsync(user.Email);
            if (userResponse == null)
                return new ResponseHandler("Login failed.");

            await _userManager.ChangePasswordAsync(userResponse, request.Request.OldPassword, request.Request.NewPassword);
            return new ResponseHandler("Password changed successfully.");
        }
    }
}
