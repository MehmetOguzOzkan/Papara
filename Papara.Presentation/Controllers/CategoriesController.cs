using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Papara.Business.Commands.CreateCategory;
using Papara.Business.Commands.DeleteCategory;
using Papara.Business.Commands.UpdateCategory;
using Papara.Business.DTOs.Category;
using Papara.Business.Queries.GetAllCategory;
using Papara.Business.Queries.GetAllCategoryFromCache;
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

        [HttpGet]
        [Authorize(Roles = "user,admin")]
        public async Task<ResponseHandler<IEnumerable<CategoryResponse>>> GetAll()
        {
            var operation = new GetAllCategoryQuery();
            var result = await _mediator.Send(operation);
            return result;
        }

        [HttpGet("Cache")]
        public async Task<ResponseHandler<IEnumerable<CategoryResponse>>> GetAllFromCache()
        {
            var operation = new GetAllCategoryFromCacheQuery();
            var result = await _mediator.Send(operation);
            return result;
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "user,admin")]
        [ResponseCache(Duration = 10000, Location = ResponseCacheLocation.Any)]
        public async Task<ResponseHandler<CategoryResponse>> Get(Guid id)
        {
            var operation = new GetCategoryByIdQuery(id);
            var result = await _mediator.Send(operation);
            return result;
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ResponseHandler<CategoryResponse>> Post([FromBody] CategoryRequest value)
        {
            var operation = new CreateCategoryCommand(value);
            var result = await _mediator.Send(operation);
            return result;
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ResponseHandler<CategoryResponse>> Put(Guid id, [FromBody] CategoryRequest value)
        {
            var operation = new UpdateCategoryCommand(id, value);
            var result = await _mediator.Send(operation);
            return result;
        }

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
