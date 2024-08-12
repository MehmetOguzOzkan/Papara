using FluentValidation.TestHelper;
using Moq;
using Papara.Business.DTOs.Payment;
using Papara.Business.Validation;
using System;
using Xunit;

namespace Papara.Test.Validation
{
    public class PaymentRequestWithoutCardValidatorTests
    {
        private readonly PaymentRequestWithoutCardValidator _validator;

        public PaymentRequestWithoutCardValidatorTests()
        {
            _validator = new PaymentRequestWithoutCardValidator();
        }

        [Fact]
        public void Should_Have_Error_When_OrderId_Is_Empty()
        {
            var model = new PaymentRequestWithoutCard { OrderId = Guid.Empty };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.OrderId);
        }

        [Fact]
        public void Should_Have_Error_When_CouponCode_Is_Invalid()
        {
            var model = new PaymentRequestWithoutCard { CouponCode = "INVALIDCODE12345" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.CouponCode);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Request_Is_Valid()
        {
            var model = new PaymentRequestWithoutCard
            {
                OrderId = Guid.NewGuid(),
                CouponCode = "VALIDCODE"
            };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.OrderId);
            result.ShouldNotHaveValidationErrorFor(x => x.CouponCode);
        }
    }
}
