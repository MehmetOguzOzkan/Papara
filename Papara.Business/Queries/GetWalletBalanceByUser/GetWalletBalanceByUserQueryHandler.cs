using MediatR;
using Microsoft.AspNetCore.Identity;
using Papara.Business.Response;
using Papara.Business.Session;
using Papara.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papara.Business.Queries.GetWalletBalanceByUser
{
    internal class GetWalletBalanceByUserQueryHandler : IRequestHandler<GetWalletBalanceByUserQuery, ResponseHandler<object>>
    {
        private readonly UserManager<User> _userManager;
        private readonly ISessionContext _sessionContext;

        public GetWalletBalanceByUserQueryHandler(UserManager<User> userManager, ISessionContext sessionContext)
        {
            _userManager = userManager;
            _sessionContext = sessionContext;
        }

        public async Task<ResponseHandler<object>> Handle(GetWalletBalanceByUserQuery request, CancellationToken cancellationToken)
        {
            User user = await _userManager.GetUserAsync(_sessionContext.HttpContext.User);
            if (user is not null)
            {
                var responseData = new
                {
                    WalletBalance = user.WalletBalance
                };

                return new ResponseHandler<object>(responseData);
            }

            return new ResponseHandler<object>("User not found");
        }
    }
}
