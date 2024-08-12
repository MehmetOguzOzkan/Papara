using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Papara.Business.DTOs.Product;
using Papara.Business.Queries.GetAllProduct;
using Papara.Business.Response;
using Papara.Data.Entities;
using Papara.Data.UnitOfWork;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Papara.Test.Queries.GetAllProduct
{
    public class GetAllProductQueryHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IMemoryCache> _memoryCacheMock;
        private readonly GetAllProductQueryHandler _handler;

        public GetAllProductQueryHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _memoryCacheMock = new Mock<IMemoryCache>();
            _handler = new GetAllProductQueryHandler(
                _unitOfWorkMock.Object,
                _mapperMock.Object,
                _memoryCacheMock.Object
            );
        }

        [Fact]
        public async Task Handle_CachedData_ReturnsCachedResponse()
        {
            // Arrange
            var cachedResponse = new ResponseHandler<IEnumerable<ProductResponse>>(new List<ProductResponse>());
            _memoryCacheMock.Setup(mc => mc.TryGetValue("ProductList", out cachedResponse)).Returns(true);

            // Act
            var result = await _handler.Handle(new GetAllProductQuery(), CancellationToken.None);

            // Assert
            Assert.Same(cachedResponse, result);
        }

        [Fact]
        public async Task Handle_NoCachedData_FetchesFromRepositoryAndCachesResult()
        {
            // Arrange
            var products = new List<Product> { new Product() };
            var productResponses = new List<ProductResponse> { new ProductResponse() };
            var response = new ResponseHandler<IEnumerable<ProductResponse>>(productResponses);

            _memoryCacheMock.Setup(mc => mc.TryGetValue("ProductList", out It.Ref<ResponseHandler<IEnumerable<ProductResponse>>>.IsAny)).Returns(false);
            _unitOfWorkMock.Setup(uow => uow.ProductRepository.GetAll($"{nameof(Product.ProductCategories)}.{nameof(ProductCategory.Category)}")).ReturnsAsync(products);
            _mapperMock.Setup(m => m.Map<IEnumerable<ProductResponse>>(products)).Returns(productResponses);
            _memoryCacheMock.Setup(mc => mc.Set("ProductList", response, It.IsAny<MemoryCacheEntryOptions>()));

            // Act
            var result = await _handler.Handle(new GetAllProductQuery(), CancellationToken.None);

            // Assert
            Assert.Equal(response, result);
            _memoryCacheMock.Verify(mc => mc.Set("ProductList", response, It.IsAny<MemoryCacheEntryOptions>()), Times.Once);
        }
    }
}
