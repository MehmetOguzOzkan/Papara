using MediatR;
using Papara.Business.DTOs.Authorization;
using Papara.Business.DTOs.User;
using Papara.Business.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Papara.Business.Commands.UpdateUser
{
    public record UpdateUserCommand(Guid Id, UserRequest Request) : IRequest<ResponseHandler>;
}
