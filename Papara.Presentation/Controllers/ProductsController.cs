using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Papara.Business.Commands.CreateProduct;
using Papara.Business.Commands.DeleteProduct;
using Papara.Business.Commands.UpdateProduct;
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

        /// <summary>
        /// Retrieves all products.
        /// </summary>
        /// <remarks>
        /// This endpoint allows users to retrieve a list of all products in the system.
        /// </remarks>
        /// <response code="200">Returns a list of all products.</response>
        [HttpGet]
        [Authorize(Roles = "user,admin")]
        public async Task<ResponseHandler<IEnumerable<ProductResponse>>> GetAll()
        {
            var operation = new GetAllProductQuery();
            var result = await _mediator.Send(operation);
            return result;
        }

        /// <summary>
        /// Retrieves a product by its unique identifier.
        /// </summary>
        /// <remarks>
        /// This endpoint allows users to retrieve the details of a product by its ID.
        /// </remarks>
        /// <param name="id">The unique identifier of the product.</param>
        /// <response code="200">Returns the product details.</response>
        /// <response code="404">Product not found.</response>
        [HttpGet("{id}")]
        [Authorize(Roles = "user,admin")]
        [ResponseCache(Duration = 10000, Location = ResponseCacheLocation.Any)]
        public async Task<ResponseHandler<ProductResponse>> Get(Guid id)
        {
            var operation = new GetProductByIdQuery(id);
            var result = await _mediator.Send(operation);
            return result;
        }

        /// <summary>
        /// Retrieves all products within a specific category.
        /// </summary>
        /// <remarks>
        /// This endpoint allows users to retrieve products that belong to a specified category.
        /// </remarks>
        /// <param name="categoryId">The unique identifier of the category.</param>
        /// <response code="200">Returns a list of products for the specified category.</response>
        /// <response code="404">Category not found or no products available.</response>
        [HttpGet("Categories/{categoryId}")]
        [Authorize(Roles = "user,admin")]
        [ResponseCache(Duration = 10000, Location = ResponseCacheLocation.Any)]
        public async Task<ResponseHandler<IEnumerable<ProductResponse>>> GetAllByCategory(Guid categoryId)
        {
            var operation = new GetAllProductByCategoryQuery(categoryId);
            var result = await _mediator.Send(operation);
            return result;
        }

        /// <summary>
        /// Creates a new product.
        /// </summary>
        /// <remarks>
        /// This endpoint allows administrators to create a new product in the system.
        /// </remarks>
        /// <param name="value">The product data to be created.</param>
        /// <response code="201">Returns the created product details.</response>
        /// <response code="400">Invalid product data.</response>
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ResponseHandler<ProductResponse>> Post([FromBody] ProductRequest value)
        {
            var operation = new CreateProductCommand(value);
            var result = await _mediator.Send(operation);
            return result;
        }

        /// <summary>
        /// Updates an existing product.
        /// </summary>
        /// <remarks>
        /// This endpoint allows administrators to update the details of an existing product.
        /// </remarks>
        /// <param name="id">The unique identifier of the product to be updated.</param>
        /// <param name="value">The updated product data.</param>
        /// <response code="200">Returns the updated product details.</response>
        /// <response code="400">Invalid product data.</response>
        /// <response code="404">Product not found.</response>
        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ResponseHandler<ProductResponse>> Put(Guid id, [FromBody] ProductRequest value)
        {
            var operation = new UpdateProductCommand(id, value);
            var result = await _mediator.Send(operation);
            return result;
        }

        /// <summary>
        /// Deletes a product.
        /// </summary>
        /// <remarks>
        /// This endpoint allows administrators to delete a product from the system.
        /// </remarks>
        /// <param name="id">The unique identifier of the product to be deleted.</param>
        /// <response code="204">Product successfully deleted.</response>
        /// <response code="404">Product not found.</response>
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
