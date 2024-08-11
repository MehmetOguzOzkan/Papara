using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Papara.Business.Commands.CreateCategory;
using Papara.Business.Commands.DeleteCategory;
using Papara.Business.Commands.UpdateCategory;
using Papara.Business.DTOs.Category;
using Papara.Business.Queries.GetAllCategory;
using Papara.Business.Queries.GetCategoryById;
using Papara.Business.Response;

namespace Papara.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CategoriesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Retrieves all categories from the database.
        /// </summary>
        /// <returns>A list of all categories.</returns>
        /// <remarks>
        /// This endpoint returns a collection of all categories available in the system. 
        /// Only users with "user" or "admin" roles are authorized to access this endpoint.
        /// </remarks>
        [HttpGet]
        [Authorize(Roles = "user,admin")]
        public async Task<ResponseHandler<IEnumerable<CategoryResponse>>> GetAll()
        {
            var operation = new GetAllCategoryQuery();
            var result = await _mediator.Send(operation);
            return result;
        }

        /// <summary>
        /// Retrieves a specific category by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the category.</param>
        /// <returns>The details of the specified category.</returns>
        /// <remarks>
        /// This endpoint allows users to retrieve detailed information about a specific category using its unique identifier.
        /// It includes caching to optimize performance.
        /// </remarks>
        [HttpGet("{id}")]
        [Authorize(Roles = "user,admin")]
        [ResponseCache(Duration = 10000, Location = ResponseCacheLocation.Any)]
        public async Task<ResponseHandler<CategoryResponse>> Get(Guid id)
        {
            var operation = new GetCategoryByIdQuery(id);
            var result = await _mediator.Send(operation);
            return result;
        }

        /// <summary>
        /// Creates a new category.
        /// </summary>
        /// <param name="value">The request containing the category details.</param>
        /// <returns>The details of the created category.</returns>
        /// <remarks>
        /// This endpoint allows administrators to create a new category by providing the necessary information.
        /// Only users with the "admin" role are authorized to perform this action.
        /// </remarks>
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ResponseHandler<CategoryResponse>> Post([FromBody] CategoryRequest value)
        {
            var operation = new CreateCategoryCommand(value);
            var result = await _mediator.Send(operation);
            return result;
        }

        /// <summary>
        /// Updates an existing category.
        /// </summary>
        /// <param name="id">The unique identifier of the category to be updated.</param>
        /// <param name="value">The updated category details.</param>
        /// <returns>The updated category information.</returns>
        /// <remarks>
        /// This endpoint allows administrators to update the details of an existing category by specifying its unique identifier and the updated information.
        /// Only users with the "admin" role are authorized to perform this action.
        /// </remarks>
        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ResponseHandler<CategoryResponse>> Put(Guid id, [FromBody] CategoryRequest value)
        {
            var operation = new UpdateCategoryCommand(id, value);
            var result = await _mediator.Send(operation);
            return result;
        }

        /// <summary>
        /// Deletes an existing category.
        /// </summary>
        /// <param name="id">The unique identifier of the category to be deleted.</param>
        /// <returns>A response indicating whether the deletion was successful.</returns>
        /// <remarks>
        /// This endpoint allows administrators to delete an existing category by specifying its unique identifier.
        /// Only users with the "admin" role are authorized to perform this action.
        /// </remarks>
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ResponseHandler> Delete(Guid id)
        {
            var operation = new DeleteCategoryCommand(id);
            var result = await _mediator.Send(operation);
            return result;
        }
    }
}
