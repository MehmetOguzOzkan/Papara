using FluentValidation.TestHelper;
using Moq;
using Papara.Business.DTOs.Authorization;
using Papara.Business.Validation;
using System;
using Xunit;

namespace Papara.Test.Validation
{
    public class AuthorizationRequestValidatorTests
    {
        private readonly AuthorizationRequestValidator _validator;

        public AuthorizationRequestValidatorTests()
        {
            _validator = new AuthorizationRequestValidator();
        }

        [Fact]
        public void Should_Have_Error_When_UserName_Is_Empty()
        {
            var model = new AuthorizationRequest { UserName = string.Empty };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.UserName);
        }

        [Fact]
        public void Should_Have_Error_When_Password_Is_Less_Than_6_Characters()
        {
            var model = new AuthorizationRequest { Password = "12345" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Password);
        }

        [Fact]
        public void Should_Not_Have_Error_When_UserName_And_Password_Are_Valid()
        {
            var model = new AuthorizationRequest { UserName = "validUser", Password = "validPass123" };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.UserName);
            result.ShouldNotHaveValidationErrorFor(x => x.Password);
        }
    }
}
