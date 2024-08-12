using FluentValidation.TestHelper;
using Moq;
using Papara.Business.DTOs.Payment;
using Papara.Business.DTOs.User;
using Papara.Business.Validation;
using System;
using Xunit;

namespace Papara.Test.Validation
{
    public class MoneyTransferRequestValidatorTests
    {
        private readonly MoneyTransferRequestValidator _validator;

        public MoneyTransferRequestValidatorTests()
        {
            _validator = new MoneyTransferRequestValidator();
        }

        [Fact]
        public void Should_Have_Error_When_Card_Is_Null()
        {
            var model = new MoneyTransferRequest { Card = null };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Card);
        }

        [Fact]
        public void Should_Have_Error_When_Amount_Is_Less_Than_Or_Equal_To_Zero()
        {
            var model = new MoneyTransferRequest { Amount = 0 };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Amount);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Request_Is_Valid()
        {
            var model = new MoneyTransferRequest
            {
                Card = new Card { CardNumber = "1234567812345678", Cvc = "123", ExpireMonth = "12", ExpireYear = "24" },
                Amount = 100
            };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Card);
            result.ShouldNotHaveValidationErrorFor(x => x.Amount);
        }
    }
}
