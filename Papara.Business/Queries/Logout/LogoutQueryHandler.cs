using MediatR;
using Microsoft.AspNetCore.Identity;
using Papara.Business.Response;
using Papara.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papara.Business.Queries.Logout
{
    internal class LogoutQueryHandler : IRequestHandler<LogoutQuery, ResponseHandler>
    {
        private readonly SignInManager<User> _signInManager;

        public LogoutQueryHandler(SignInManager<User> signInManager)
        {
            _signInManager = signInManager;
        }

        public async Task<ResponseHandler> Handle(LogoutQuery request, CancellationToken cancellationToken)
        {
            await _signInManager.SignOutAsync();
            return new ResponseHandler("Logout successfully.");
        }
    }
}
