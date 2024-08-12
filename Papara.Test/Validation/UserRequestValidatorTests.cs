using FluentValidation.TestHelper;
using Moq;
using Papara.Business.DTOs.User;
using Papara.Business.Validation;
using System;
using Xunit;

namespace Papara.Test.Validation
{
    public class UserRequestValidatorTests
    {
        private readonly UserRequestValidator _validator;

        public UserRequestValidatorTests()
        {
            _validator = new UserRequestValidator();
        }

        [Fact]
        public void Should_Have_Error_When_FirstName_Is_Empty()
        {
            var model = new UserRequest { FirstName = string.Empty };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.FirstName);
        }

        [Fact]
        public void Should_Have_Error_When_Email_Is_Invalid()
        {
            var model = new UserRequest { Email = "invalidEmail" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Request_Is_Valid()
        {
            var model = new UserRequest
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com"
            };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.FirstName);
            result.ShouldNotHaveValidationErrorFor(x => x.LastName);
            result.ShouldNotHaveValidationErrorFor(x => x.Email);
        }
    }
}
