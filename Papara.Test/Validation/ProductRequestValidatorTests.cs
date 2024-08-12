using FluentValidation.TestHelper;
using Moq;
using Papara.Business.DTOs.Product;
using Papara.Business.Validation;
using System;
using Xunit;

namespace Papara.Test.Validation
{
    public class ProductRequestValidatorTests
    {
        private readonly ProductRequestValidator _validator;

        public ProductRequestValidatorTests()
        {
            _validator = new ProductRequestValidator();
        }

        [Fact]
        public void Should_Have_Error_When_Name_Is_Empty()
        {
            var model = new ProductRequest { Name = string.Empty };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_Have_Error_When_Name_Exceeds_MaxLength()
        {
            var model = new ProductRequest { Name = new string('A', 201) };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_Have_Error_When_Price_Is_Less_Than_Or_Equal_To_Zero()
        {
            var model = new ProductRequest { Price = 0 };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Price);
        }

        [Fact]
        public void Should_Have_Error_When_LoyaltyPointsRatio_Is_Out_Of_Range()
        {
            var model = new ProductRequest { LoyaltyPointsRatio = 1.5m };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.LoyaltyPointsRatio);
        }

        [Fact]
        public void Should_Have_Error_When_MaxPoints_Is_Less_Than_Or_Equal_To_Zero()
        {
            var model = new ProductRequest { MaxPoints = 0 };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.MaxPoints);
        }

        [Fact]
        public void Should_Have_Error_When_CategoryIds_Is_Empty()
        {
            var model = new ProductRequest { CategoryIds = new List<Guid>() };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.CategoryIds);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Request_Is_Valid()
        {
            var model = new ProductRequest
            {
                Name = "Valid Product",
                Description = "Valid description",
                Price = 10,
                LoyaltyPointsRatio = 0.5m,
                MaxPoints = 100,
                CategoryIds = new List<Guid> { Guid.NewGuid() }
            };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.Name);
            result.ShouldNotHaveValidationErrorFor(x => x.Description);
            result.ShouldNotHaveValidationErrorFor(x => x.Price);
            result.ShouldNotHaveValidationErrorFor(x => x.LoyaltyPointsRatio);
            result.ShouldNotHaveValidationErrorFor(x => x.MaxPoints);
            result.ShouldNotHaveValidationErrorFor(x => x.CategoryIds);
        }
    }
}
