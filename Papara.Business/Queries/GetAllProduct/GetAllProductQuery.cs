using MediatR;
using Papara.Business.DTOs.Product;
using Papara.Business.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papara.Business.Queries.GetAllProduct
{
    public record GetAllProductQuery() : IRequest<ResponseHandler<IEnumerable<ProductResponse>>>;
}
