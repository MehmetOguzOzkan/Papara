using MediatR;
using Papara.Business.DTOs.Category;
using Papara.Business.DTOs.Product;
using Papara.Business.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papara.Business.Commands.CreateProduct
{
    public record CreateProductCommand(ProductRequest Request) : IRequest<ResponseHandler<ProductResponse>>;

}
