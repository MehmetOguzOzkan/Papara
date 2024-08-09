using MediatR;
using Papara.Business.DTOs.Category;
using Papara.Business.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papara.Business.Commands.CreateCategory
{
    public record CreateCategoryCommand(CategoryRequest Request) : IRequest<ResponseHandler<CategoryResponse>>;
}
