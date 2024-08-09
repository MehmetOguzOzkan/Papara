using MediatR;
using Papara.Business.DTOs.Authorization;
using Papara.Business.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papara.Business.Commands.ChangePassword
{
    public record ChangePasswordCommand(ChangePasswordRequest Request) : IRequest<ResponseHandler>;
}
