using FluentValidation.TestHelper;
using Moq;
using Papara.Business.DTOs.Payment;
using Papara.Business.Validation;
using System;
using Xunit;

namespace Papara.Test.Validation
{
    public class CardValidatorTests
    {
        private readonly CardValidator _validator;

        public CardValidatorTests()
        {
            _validator = new CardValidator();
        }

        [Fact]
        public void Should_Have_Error_When_CardNumber_Is_Not_16_Digits()
        {
            var model = new Card { CardNumber = "123456789012345" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.CardNumber);
        }

        [Fact]
        public void Should_Have_Error_When_ExpireMonth_Is_Invalid()
        {
            var model = new Card { ExpireMonth = "13" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.ExpireMonth);
        }

        [Fact]
        public void Should_Have_Error_When_ExpireDate_Is_In_The_Past()
        {
            var model = new Card { ExpireMonth = "01", ExpireYear = "24" }; // Assuming the test runs after August 2022
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Card_Is_Valid()
        {
            var model = new Card { CardNumber = "1234567812345678", Cvc = "123", ExpireMonth = "12", ExpireYear = "24" };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.CardNumber);
            result.ShouldNotHaveValidationErrorFor(x => x.Cvc);
            result.ShouldNotHaveValidationErrorFor(x => x.ExpireMonth);
            result.ShouldNotHaveValidationErrorFor(x => x.ExpireYear);
        }
    }
}
