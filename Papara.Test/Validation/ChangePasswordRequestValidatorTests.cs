using FluentValidation.TestHelper;
using Moq;
using Papara.Business.DTOs.Authorization;
using Papara.Business.Validation;
using System;
using Xunit;

namespace Papara.Test.Validation
{
    public class ChangePasswordRequestValidatorTests
    {
        private readonly ChangePasswordRequestValidator _validator;

        public ChangePasswordRequestValidatorTests()
        {
            _validator = new ChangePasswordRequestValidator();
        }

        [Fact]
        public void Should_Have_Error_When_OldPassword_Is_Empty()
        {
            var model = new ChangePasswordRequest { OldPassword = string.Empty };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.OldPassword);
        }

        [Fact]
        public void Should_Have_Error_When_NewPassword_Is_Less_Than_6_Characters()
        {
            var model = new ChangePasswordRequest { NewPassword = "12345" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.NewPassword);
        }

        [Fact]
        public void Should_Have_Error_When_NewPassword_Is_Same_As_OldPassword()
        {
            var model = new ChangePasswordRequest { OldPassword = "oldpass", NewPassword = "oldpass" };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.NewPassword);
        }

        [Fact]
        public void Should_Not_Have_Error_When_NewPassword_Is_Valid()
        {
            var model = new ChangePasswordRequest { OldPassword = "oldpass", NewPassword = "newpass123" };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.OldPassword);
            result.ShouldNotHaveValidationErrorFor(x => x.NewPassword);
        }
    }
}
