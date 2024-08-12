using FluentValidation.TestHelper;
using Moq;
using Papara.Business.DTOs.Order;
using Papara.Business.DTOs.OrderDetail;
using Papara.Business.Validation;
using System;
using Xunit;

namespace Papara.Test.Validation
{
    public class OrderRequestValidatorTests
    {
        private readonly OrderRequestValidator _validator;

        public OrderRequestValidatorTests()
        {
            _validator = new OrderRequestValidator();
        }

        [Fact]
        public void Should_Have_Error_When_OrderDetails_Is_Empty()
        {
            var model = new OrderRequest { OrderDetails = new List<OrderDetailRequest>() };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.OrderDetails);
        }

        [Fact]
        public void Should_Have_Error_When_OrderDetail_Is_Invalid()
        {
            var model = new OrderRequest
            {
                OrderDetails = new List<OrderDetailRequest>
            {
                new OrderDetailRequest { ProductId = Guid.Empty, Quantity = 0 }
            }
            };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.OrderDetails);
        }

        [Fact]
        public void Should_Not_Have_Error_When_OrderDetails_Are_Valid()
        {
            var model = new OrderRequest
            {
                OrderDetails = new List<OrderDetailRequest>
            {
                new OrderDetailRequest { ProductId = Guid.NewGuid(), Quantity = 5 }
            }
            };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.OrderDetails);
        }
    }
}
