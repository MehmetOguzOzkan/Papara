using MediatR;
using Papara.Business.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papara.Business.Commands.DeleteCategory
{
    public record DeleteCategoryCommand(Guid Id) : IRequest<ResponseHandler>;
}
