using FluentValidation.TestHelper;
using Moq;
using Papara.Business.DTOs.Payment;
using Papara.Business.Validation;
using System;
using Xunit;

namespace Papara.Test.Validation
{
    public class PaymentRequestValidatorTests
    {
        private readonly PaymentRequestValidator _validator;

        public PaymentRequestValidatorTests()
        {
            _validator = new PaymentRequestValidator();
        }

        [Fact]
        public void Should_Have_Error_When_Card_Is_Null()
        {
            var model = new PaymentRequest { Card = null };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Card);
        }

        [Fact]
        public void Should_Have_Error_When_OrderId_Is_Empty()
        {
            var model = new PaymentRequest { OrderId = Guid.Empty };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.OrderId);
        }

        [Fact]
        public void Should_Have_Error_When_CouponCode_Is_Invalid()
        {
            var model = new PaymentRequest { CouponCode = "INVALIDCODE12345" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.CouponCode);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Request_Is_Valid()
        {
            var model = new PaymentRequest
            {
                Card = new Card { CardNumber = "1234567812345678", Cvc = "123", ExpireMonth = "12", ExpireYear = "24" },
                OrderId = Guid.NewGuid(),
                CouponCode = "VALIDCODE"
            };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Card);
            result.ShouldNotHaveValidationErrorFor(x => x.OrderId);
            result.ShouldNotHaveValidationErrorFor(x => x.CouponCode);
        }
    }
}
