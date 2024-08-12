using AutoMapper;
using FluentValidation;
using Moq;
using Papara.Business.Commands.CreateCoupon;
using Papara.Business.DTOs.Coupon;
using Papara.Data.Entities;
using Papara.Data.UnitOfWork;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Papara.Test.Commands.CreateCoupon
{
    public class CreateCouponCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IValidator<CouponRequest>> _validatorMock;
        private readonly CreateCouponCommandHandler _handler;

        public CreateCouponCommandHandlerTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _validatorMock = new Mock<IValidator<CouponRequest>>();

            _handler = new CreateCouponCommandHandler(
                _unitOfWorkMock.Object,
                _mapperMock.Object,
                _validatorMock.Object
            );
        }

        [Fact]
        public async Task Handle_ValidRequest_ReturnsSuccessResponse()
        {
            // Arrange
            var couponRequest = new CouponRequest
            {
                DiscountAmount = 10.0m,
                ValidFrom = DateTime.UtcNow,
                ValidTo = DateTime.UtcNow.AddDays(30),
                UserId = Guid.NewGuid()
            };
            var command = new CreateCouponCommand(couponRequest);
            var coupon = new Coupon { DiscountAmount = 10.0m, Code = "ABC123" };
            var couponResponse = new CouponResponse { DiscountAmount = 10.0m, Code = "ABC123" };

            _validatorMock.Setup(v => v.ValidateAsync(couponRequest, It.IsAny<CancellationToken>()))
                          .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            _mapperMock.Setup(m => m.Map<Coupon>(couponRequest)).Returns(coupon);
            _mapperMock.Setup(m => m.Map<CouponResponse>(coupon)).Returns(couponResponse);

            _unitOfWorkMock.Setup(uow => uow.CouponRepository.Insert(It.IsAny<Coupon>()))
                           .ReturnsAsync(coupon); // Adjusted to return Coupon
            _unitOfWorkMock.Setup(uow => uow.CompleteWithTransaction()).Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal("ABC123", result.Data.Code);
            Assert.Equal(10.0m, result.Data.DiscountAmount);
        }

        [Fact]
        public async Task Handle_InvalidRequest_ReturnsErrorResponse()
        {
            // Arrange
            var couponRequest = new CouponRequest
            {
                DiscountAmount = 0, // Invalid DiscountAmount
                ValidFrom = DateTime.UtcNow,
                ValidTo = DateTime.UtcNow.AddDays(30),
                UserId = Guid.NewGuid()
            };
            var command = new CreateCouponCommand(couponRequest);

            _validatorMock.Setup(v => v.ValidateAsync(couponRequest, It.IsAny<CancellationToken>()))
                          .ReturnsAsync(new FluentValidation.Results.ValidationResult(new[]
                          {
                          new FluentValidation.Results.ValidationFailure("DiscountAmount", "DiscountAmount must be greater than 0.")
                          }));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Contains("DiscountAmount must be greater than 0.", result.Message);
        }
    }
}
