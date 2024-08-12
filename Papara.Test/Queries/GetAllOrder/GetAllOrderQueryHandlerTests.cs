using AutoMapper;
using Moq;
using Papara.Business.DTOs.Order;
using Papara.Business.DTOs.OrderDetail;
using Papara.Business.Queries.GetAllOrder;
using Papara.Data.Entities;
using Papara.Data.UnitOfWork;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Papara.Test.Queries.GetAllOrder
{
    public class GetAllOrderQueryHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly GetAllOrderQueryHandler _handler;

        public GetAllOrderQueryHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _handler = new GetAllOrderQueryHandler(
                _unitOfWorkMock.Object,
                _mapperMock.Object
            );
        }

        [Fact]
        public async Task Handle_ValidRequest_ReturnsMappedOrderResponses()
        {
            // Arrange
            var orders = new List<Order> { new Order { OrderDetails = new List<OrderDetail> { new OrderDetail() } } };
            var orderResponses = new List<OrderResponse> { new OrderResponse { OrderDetails = new List<OrderDetailResponse> { new OrderDetailResponse() } } };

            _unitOfWorkMock.Setup(uow => uow.OrderRepository.GetAll(nameof(Order.User), nameof(Order.OrderDetails) + "." + nameof(OrderDetail.Product)))
                .ReturnsAsync(orders);
            _mapperMock.Setup(m => m.Map<OrderResponse>(It.IsAny<Order>())).Returns(new OrderResponse());
            _mapperMock.Setup(m => m.Map<OrderDetailResponse>(It.IsAny<OrderDetail>())).Returns(new OrderDetailResponse());

            // Act
            var result = await _handler.Handle(new GetAllOrderQuery(), CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(orderResponses.Count, result.Data.Count());
        }
    }
}
