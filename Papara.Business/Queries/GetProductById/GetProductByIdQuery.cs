using MediatR;
using Papara.Business.DTOs.Product;
using Papara.Business.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papara.Business.Queries.GetProductById
{
    public record GetProductByIdQuery(Guid Id) : IRequest<ResponseHandler<ProductResponse>>; 
}
