using FluentValidation.TestHelper;
using Moq;
using Papara.Business.DTOs.OrderDetail;
using Papara.Business.Validation;
using System;
using Xunit;

namespace Papara.Test.Validation
{
    public class OrderDetailRequestValidatorTests
    {
        private readonly OrderDetailRequestValidator _validator;

        public OrderDetailRequestValidatorTests()
        {
            _validator = new OrderDetailRequestValidator();
        }

        [Fact]
        public void Should_Have_Error_When_ProductId_Is_Empty()
        {
            var model = new OrderDetailRequest { ProductId = Guid.Empty };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.ProductId);
        }

        [Fact]
        public void Should_Have_Error_When_Quantity_Is_Zero()
        {
            var model = new OrderDetailRequest { Quantity = 0 };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Quantity);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Request_Is_Valid()
        {
            var model = new OrderDetailRequest { ProductId = Guid.NewGuid(), Quantity = 5 };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.ProductId);
            result.ShouldNotHaveValidationErrorFor(x => x.Quantity);
        }
    }
}
