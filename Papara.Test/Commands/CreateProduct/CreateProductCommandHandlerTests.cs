using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Papara.Business.Commands.CreateProduct;
using Papara.Business.DTOs.Product;
using Papara.Data.Entities;
using Papara.Data.UnitOfWork;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Papara.Test.Commands.CreateProduct
{
    public class CreateProductCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IMemoryCache> _memoryCacheMock;
        private readonly Mock<IDistributedCache> _distributedCacheMock;
        private readonly Mock<IValidator<ProductRequest>> _validatorMock;
        private readonly CreateProductCommandHandler _handler;

        public CreateProductCommandHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _memoryCacheMock = new Mock<IMemoryCache>();
            _distributedCacheMock = new Mock<IDistributedCache>();
            _validatorMock = new Mock<IValidator<ProductRequest>>();

            _handler = new CreateProductCommandHandler(
                _unitOfWorkMock.Object,
                _mapperMock.Object,
                _memoryCacheMock.Object,
                _distributedCacheMock.Object,
                _validatorMock.Object
            );
        }

        [Fact]
        public async Task Handle_ValidRequest_ReturnsSuccessResponse()
        {
            // Arrange
            var productRequest = new ProductRequest
            {
                Name = "Test Product",
                CategoryIds = new List<Guid> { Guid.NewGuid() }
            };
            var command = new CreateProductCommand(productRequest);
            var product = new Product { Id = Guid.NewGuid(), Name = "Test Product" };
            var productResponse = new ProductResponse { Id = product.Id, Name = "Test Product" };

            _validatorMock.Setup(v => v.ValidateAsync(productRequest, It.IsAny<CancellationToken>()))
                          .ReturnsAsync(new FluentValidation.Results.ValidationResult());
            _mapperMock.Setup(m => m.Map<Product>(productRequest)).Returns(product);
            _mapperMock.Setup(m => m.Map<ProductResponse>(product)).Returns(productResponse);
            _unitOfWorkMock.Setup(uow => uow.ProductRepository.Insert(product)).ReturnsAsync(product);
            _unitOfWorkMock.Setup(uow => uow.CompleteWithTransaction()).Returns(Task.CompletedTask);
            _memoryCacheMock.Setup(mc => mc.Remove("ProductList"));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal("Test Product", result.Data.Name);
        }

        [Fact]
        public async Task Handle_InvalidRequest_ReturnsErrorResponse()
        {
            // Arrange
            var productRequest = new ProductRequest
            {
                Name = "" // Invalid request
            };
            var command = new CreateProductCommand(productRequest);

            _validatorMock.Setup(v => v.ValidateAsync(productRequest, It.IsAny<CancellationToken>()))
                          .ReturnsAsync(new FluentValidation.Results.ValidationResult(new[]
                          {
                          new FluentValidation.Results.ValidationFailure("Name", "Name is required.")
                          }));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Contains("Name is required.", result.Message);
        }
    }
}
