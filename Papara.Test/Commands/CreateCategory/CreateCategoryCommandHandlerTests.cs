using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Papara.Business.Commands.CreateCategory;
using Papara.Business.DTOs.Category;
using Papara.Data.Entities;
using Papara.Data.UnitOfWork;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Papara.Test.Commands.CreateCategory
{
    public class CreateCategoryCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IMemoryCache> _memoryCacheMock;
        private readonly Mock<IDistributedCache> _distributedCacheMock;
        private readonly Mock<IValidator<CategoryRequest>> _validatorMock;
        private readonly CreateCategoryCommandHandler _handler;

        public CreateCategoryCommandHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _memoryCacheMock = new Mock<IMemoryCache>();
            _distributedCacheMock = new Mock<IDistributedCache>();
            _validatorMock = new Mock<IValidator<CategoryRequest>>();

            _handler = new CreateCategoryCommandHandler(
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
            var categoryRequest = new CategoryRequest
            {
                Name = "Test Category"
            };
            var command = new CreateCategoryCommand(categoryRequest);
            var category = new Category { Name = "Test Category" };
            var categoryResponse = new CategoryResponse { Name = "Test Category", Url = "test-category" };

            _validatorMock.Setup(v => v.ValidateAsync(categoryRequest, It.IsAny<CancellationToken>()))
                          .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            _mapperMock.Setup(m => m.Map<Category>(categoryRequest)).Returns(category);
            _mapperMock.Setup(m => m.Map<CategoryResponse>(category)).Returns(categoryResponse);

            _unitOfWorkMock.Setup(uow => uow.CategoryRepository.Insert(It.IsAny<Category>()))
                           .ReturnsAsync(category); // Adjusted to return Category
            _unitOfWorkMock.Setup(uow => uow.CompleteWithTransaction()).Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal("test-category", result.Data.Url);
            Assert.Equal("Test Category", result.Data.Name);
        }

        [Fact]
        public async Task Handle_InvalidRequest_ReturnsErrorResponse()
        {
            // Arrange
            var categoryRequest = new CategoryRequest
            {
                Name = ""
            };
            var command = new CreateCategoryCommand(categoryRequest);

            _validatorMock.Setup(v => v.ValidateAsync(categoryRequest, It.IsAny<CancellationToken>()))
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
