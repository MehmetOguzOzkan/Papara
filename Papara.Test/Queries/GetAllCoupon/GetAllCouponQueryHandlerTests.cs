using AutoMapper;
using Moq;
using Papara.Business.DTOs.Coupon;
using Papara.Business.Queries.GetAllCoupon;
using Papara.Business.Response;
using Papara.Data.Entities;
using Papara.Data.UnitOfWork;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Papara.Test.Queries.GetAllCoupon
{
    public class GetAllCouponQueryHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly GetAllCouponQueryHandler _handler;

        public GetAllCouponQueryHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _handler = new GetAllCouponQueryHandler(
                _unitOfWorkMock.Object,
                _mapperMock.Object
            );
        }

        [Fact]
        public async Task Handle_ValidRequest_ReturnsMappedResponse()
        {
            // Arrange
            var coupons = new List<Coupon> { new Coupon() };
            var couponResponses = new List<CouponResponse> { new CouponResponse() };
            var response = new ResponseHandler<IEnumerable<CouponResponse>>(couponResponses);

            _unitOfWorkMock.Setup(uow => uow.CouponRepository.GetAll()).ReturnsAsync(coupons);
            _mapperMock.Setup(m => m.Map<IEnumerable<CouponResponse>>(coupons)).Returns(couponResponses);

            // Act
            var result = await _handler.Handle(new GetAllCouponsQuery(), CancellationToken.None);

            // Assert
            Assert.Equal(response, result);
        }
    }
}
