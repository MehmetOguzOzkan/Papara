using MediatR;
using Papara.Business.DTOs.Authorization;
using Papara.Business.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Papara.Business.Queries.Login
{
    public record LoginQuery(AuthorizationRequest Request) : IRequest<ResponseHandler<AuthorizationResponse>>;
}
