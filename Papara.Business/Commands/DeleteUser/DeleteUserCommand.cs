using MediatR;
using Papara.Business.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papara.Business.Commands.DeleteUser
{
    public record DeleteUserCommand(Guid Id) : IRequest<ResponseHandler>;
}
