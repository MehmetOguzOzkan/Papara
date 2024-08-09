using MediatR;
using Papara.Business.DTOs.Category;
using Papara.Business.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papara.Business.Queries.GetCategoryById
{
    public record GetCategoryByIdQuery(Guid Id) : IRequest<ResponseHandler<CategoryResponse>>;
}
