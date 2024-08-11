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

namespace Papara.Business.Queries.GetLoyaltyPointByUser
{
    internal class GetLoyaltyPointByUserQueryHandler : IRequestHandler<GetLoyaltyPointByUserQuery, ResponseHandler<object>>
    {
        private readonly UserManager<User> _userManager;
        private readonly ISessionContext _sessionContext;

        public GetLoyaltyPointByUserQueryHandler(UserManager<User> userManager, ISessionContext sessionContext)
        {
            _userManager = userManager;
            _sessionContext = sessionContext;
        }

        public async Task<ResponseHandler<object>> Handle(GetLoyaltyPointByUserQuery request, CancellationToken cancellationToken)
        {
            User user = await _userManager.GetUserAsync(_sessionContext.HttpContext.User);
            if (user is not null)
            {
                var responseData = new
                {
                    LoyaltyPointsBalance = user.PointsBalance
                };

                return new ResponseHandler<object>(responseData);
            }

            return new ResponseHandler<object>("User not found");
        }
    }
}
