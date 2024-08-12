using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Moq;
using Papara.Business.Commands.CreateOrder;
using Papara.Business.DTOs.Order;
using Papara.Business.DTOs.OrderDetail;
using Papara.Business.Session;
using Papara.Data.Entities;
using Papara.Data.UnitOfWork;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace Papara.Test.Commands.CreateOrder
{
    public class CreateOrderCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IValidator<OrderRequest>> _validatorMock;
        private readonly Mock<ISessionContext> _sessionContextMock;
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly CreateOrderCommandHandler _handler;

        public CreateOrderCommandHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _validatorMock = new Mock<IValidator<OrderRequest>>();
            _sessionContextMock = new Mock<ISessionContext>();
            _userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(),
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null
            );

            _handler = new CreateOrderCommandHandler(
                _unitOfWorkMock.Object,
                _mapperMock.Object,
                _validatorMock.Object,
                _sessionContextMock.Object,
                _userManagerMock.Object
            );
        }

        [Fact]
        public async Task Handle_ValidRequest_ReturnsSuccessResponse()
        {
            // Arrange
            var orderRequest = new OrderRequest
            {
                OrderDetails = new List<OrderDetailRequest>
            {
                new OrderDetailRequest { ProductId = Guid.NewGuid(), Quantity = 2 }
            }
            };
            var command = new CreateOrderCommand(orderRequest);
            var user = new User { Id = Guid.NewGuid() };
            var product = new Product { Price = 100.0m };
            var order = new Order
            {
                OrderDetails = new List<OrderDetail>
            {
                new OrderDetail { ProductId = product.Id, Quantity = 2 }
            }
            };
            var orderResponse = new OrderResponse { TotalAmount = 200.0m };

            _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _userManagerMock.Setup(um => um.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);
            _validatorMock.Setup(v => v.ValidateAsync(orderRequest, It.IsAny<CancellationToken>()))
                          .ReturnsAsync(new FluentValidation.Results.ValidationResult());
            _mapperMock.Setup(m => m.Map<Order>(orderRequest)).Returns(order);
            _mapperMock.Setup(m => m.Map<OrderResponse>(order)).Returns(orderResponse);
            _unitOfWorkMock.Setup(uow => uow.ProductRepository.GetById(It.IsAny<Guid>())).ReturnsAsync(product);
            _unitOfWorkMock.Setup(uow => uow.OrderRepository.Insert(It.IsAny<Order>())).ReturnsAsync(order);
            _unitOfWorkMock.Setup(uow => uow.CompleteWithTransaction()).Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(200.0m, result.Data.TotalAmount);
        }

        [Fact]
        public async Task Handle_UserNotFound_ReturnsErrorResponse()
        {
            // Arrange
            var orderRequest = new OrderRequest();
            var command = new CreateOrderCommand(orderRequest);

            _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((User)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal("Login failed.", result.Message);
        }

        [Fact]
        public async Task Handle_ProductNotFound_ReturnsErrorResponse()
        {
            // Arrange
            var orderRequest = new OrderRequest
            {
                OrderDetails = new List<OrderDetailRequest>
            {
                new OrderDetailRequest { ProductId = Guid.NewGuid(), Quantity = 2 }
            }
            };
            var command = new CreateOrderCommand(orderRequest);
            var user = new User { Id = Guid.NewGuid() };

            _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _userManagerMock.Setup(um => um.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);
            _validatorMock.Setup(v => v.ValidateAsync(orderRequest, It.IsAny<CancellationToken>()))
                          .ReturnsAsync(new FluentValidation.Results.ValidationResult());
            _unitOfWorkMock.Setup(uow => uow.ProductRepository.GetById(It.IsAny<Guid>())).ReturnsAsync((Product)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Contains("Product with ID", result.Message);
        }
    }
}
