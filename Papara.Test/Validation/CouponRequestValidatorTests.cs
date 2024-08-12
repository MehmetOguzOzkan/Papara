using FluentValidation.TestHelper;
using Moq;
using Papara.Business.DTOs.Coupon;
using Papara.Business.Validation;
using System;
using Xunit;

namespace Papara.Test.Validation
{
    public class CouponRequestValidatorTests
    {
        private readonly CouponRequestValidator _validator;

        public CouponRequestValidatorTests()
        {
            _validator = new CouponRequestValidator();
        }

        [Fact]
        public void Should_Have_Error_When_DiscountAmount_Is_Less_Than_Or_Equal_To_Zero()
        {
            var model = new CouponRequest { DiscountAmount = 0 };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.DiscountAmount);
        }

        [Fact]
        public void Should_Have_Error_When_ValidFrom_Is_After_ValidTo()
        {
            var model = new CouponRequest { ValidFrom = DateTime.Now.AddDays(1), ValidTo = DateTime.Now };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.ValidFrom);
        }

        [Fact]
        public void Should_Have_Error_When_UserId_Is_Empty()
        {
            var model = new CouponRequest { UserId = Guid.Empty };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.UserId);
        }

        [Fact]
        public void Should_Not_Have_Error_When_CouponRequest_Is_Valid()
        {
            var model = new CouponRequest
            {
                DiscountAmount = 10,
                ValidFrom = DateTime.Now,
                ValidTo = DateTime.Now.AddDays(1),
                UserId = Guid.NewGuid()
            };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.DiscountAmount);
            result.ShouldNotHaveValidationErrorFor(x => x.ValidFrom);
            result.ShouldNotHaveValidationErrorFor(x => x.ValidTo);
            result.ShouldNotHaveValidationErrorFor(x => x.UserId);
        }
    }
}
