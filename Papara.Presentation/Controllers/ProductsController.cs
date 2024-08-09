using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Papara.Business.Commands.CreateCategory;
using Papara.Business.Commands.CreateProduct;
using Papara.Business.Commands.DeleteCategory;
using Papara.Business.Commands.DeleteProduct;
using Papara.Business.Commands.UpdateCategory;
using Papara.Business.Commands.UpdateProduct;
using Papara.Business.DTOs.Category;
using Papara.Business.DTOs.Product;
using Papara.Business.Queries.GetAllProduct;
using Papara.Business.Queries.GetAllProductByCategory;
using Papara.Business.Queries.GetProductById;
using Papara.Business.Response;

namespace Papara.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Authorize(Roles = "user,admin")]
        public async Task<ResponseHandler<IEnumerable<ProductResponse>>> GetAll()
        {
            var operation = new GetAllProductQuery();
            var result = await _mediator.Send(operation);
            return result;
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "user,admin")]
        [ResponseCache(Duration = 10000, Location = ResponseCacheLocation.Any)]
        public async Task<ResponseHandler<ProductResponse>> Get(Guid id)
        {
            var operation = new GetProductByIdQuery(id);
            var result = await _mediator.Send(operation);
            return result;
        }

        [HttpGet("Categories/{categoryId}")]
        [Authorize(Roles = "user,admin")]
        [ResponseCache(Duration = 10000, Location = ResponseCacheLocation.Any)]
        public async Task<ResponseHandler<IEnumerable<ProductResponse>>> GetAllByCategory(Guid categoryId)
        {
            var operation = new GetAllProductByCategoryQuery(categoryId);
            var result = await _mediator.Send(operation);
            return result;
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ResponseHandler<ProductResponse>> Post([FromBody] ProductRequest value)
        {
            var operation = new CreateProductCommand(value);
            var result = await _mediator.Send(operation);
            return result;
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ResponseHandler<ProductResponse>> Put(Guid id, [FromBody] ProductRequest value)
        {
            var operation = new UpdateProductCommand(id, value);
            var result = await _mediator.Send(operation);
            return result;
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ResponseHandler> Delete(Guid id)
        {
            var operation = new DeleteProductCommand(id);
            var result = await _mediator.Send(operation);
            return result;
        }
    }
}
