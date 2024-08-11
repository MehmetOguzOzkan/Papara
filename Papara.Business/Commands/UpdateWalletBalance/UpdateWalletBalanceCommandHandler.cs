using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Papara.Business.DTOs.User;
using Papara.Business.Response;
using Papara.Business.Session;
using Papara.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papara.Business.Commands.UpdateWalletBalance
{
    internal class UpdateWalletBalanceCommandHandler : IRequestHandler<UpdateWalletBalanceCommand, ResponseHandler>
    {
        private readonly UserManager<User> _userManager;
        private readonly ISessionContext _sessionContext;
        private readonly IValidator<MoneyTransferRequest> _validator;

        public UpdateWalletBalanceCommandHandler(UserManager<User> userManager, ISessionContext sessionContext, IValidator<MoneyTransferRequest> validator)
        {
            _userManager = userManager;
            _sessionContext = sessionContext;
            _validator = validator;
        }

        public async Task<ResponseHandler> Handle(UpdateWalletBalanceCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request.Request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return new ResponseHandler(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
            }

            User user = await _userManager.GetUserAsync(_sessionContext.HttpContext.User);
            if (user == null)
            {
                return new ResponseHandler("Login failed.");
            }

            if(request.Request.Amount > request.Request.Card.Balance)
            {
                return new ResponseHandler("Balance is not enough.");
            }

            user.WalletBalance += request.Request.Amount;
            request.Request.Card.Balance -= request.Request.Amount;
            await _userManager.UpdateAsync(user);

            return new ResponseHandler("Money transfered successfully.");
        }
    }
}
