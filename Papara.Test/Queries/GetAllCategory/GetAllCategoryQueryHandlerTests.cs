using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Papara.Business.DTOs.Category;
using Papara.Business.Queries.GetAllCategory;
using Papara.Business.Response;
using Papara.Data.Entities;
using Papara.Data.UnitOfWork;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Papara.Test.Queries.GetAllCategory
{
    public class GetAllCategoryQueryHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IMemoryCache> _memoryCacheMock;
        private readonly GetAllCategoryQueryHandler _handler;

        public GetAllCategoryQueryHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _memoryCacheMock = new Mock<IMemoryCache>();
            _handler = new GetAllCategoryQueryHandler(
                _unitOfWorkMock.Object,
                _mapperMock.Object,
                _memoryCacheMock.Object
            );
        }

        [Fact]
        public async Task Handle_CachedData_ReturnsCachedResponse()
        {
            // Arrange
            var cachedResponse = new ResponseHandler<IEnumerable<CategoryResponse>>(new List<CategoryResponse>());
            _memoryCacheMock.Setup(mc => mc.TryGetValue("CategoryList", out cachedResponse)).Returns(true);

            // Act
            var result = await _handler.Handle(new GetAllCategoryQuery(), CancellationToken.None);

            // Assert
            Assert.Same(cachedResponse, result);
        }

        [Fact]
        public async Task Handle_NoCachedData_FetchesFromRepositoryAndCachesResult()
        {
            // Arrange
            var categories = new List<Category> { new Category() };
            var categoryResponses = new List<CategoryResponse> { new CategoryResponse() };
            var response = new ResponseHandler<IEnumerable<CategoryResponse>>(categoryResponses);

            _memoryCacheMock.Setup(mc => mc.TryGetValue("CategoryList", out It.Ref<ResponseHandler<IEnumerable<CategoryResponse>>>.IsAny)).Returns(false);
            _unitOfWorkMock.Setup(uow => uow.CategoryRepository.GetAll()).ReturnsAsync(categories);
            _mapperMock.Setup(m => m.Map<IEnumerable<CategoryResponse>>(categories)).Returns(categoryResponses);
            _memoryCacheMock.Setup(mc => mc.Set("CategoryList", response, It.IsAny<MemoryCacheEntryOptions>()));

            // Act
            var result = await _handler.Handle(new GetAllCategoryQuery(), CancellationToken.None);

            // Assert
            Assert.Equal(response, result);
            _memoryCacheMock.Verify(mc => mc.Set("CategoryList", response, It.IsAny<MemoryCacheEntryOptions>()), Times.Once);
        }
    }
}
