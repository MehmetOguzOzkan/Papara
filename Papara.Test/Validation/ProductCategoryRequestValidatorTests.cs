using FluentValidation.TestHelper;
using Moq;
using Papara.Business.DTOs.ProductCategory;
using Papara.Business.Validation;
using System;
using Xunit;

namespace Papara.Test.Validation
{
    public class ProductCategoryRequestValidatorTests
    {
        private readonly ProductCategoryRequestValidator _validator;

        public ProductCategoryRequestValidatorTests()
        {
            _validator = new ProductCategoryRequestValidator();
        }

        [Fact]
        public void Should_Have_Error_When_ProductId_Is_Empty()
        {
            var model = new ProductCategoryRequest { ProductId = Guid.Empty };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.ProductId);
        }

        [Fact]
        public void Should_Have_Error_When_CategoryId_Is_Empty()
        {
            var model = new ProductCategoryRequest { CategoryId = Guid.Empty };
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.CategoryId);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Request_Is_Valid()
        {
            var model = new ProductCategoryRequest { ProductId = Guid.NewGuid(), CategoryId = Guid.NewGuid() };
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.ProductId);
            result.ShouldNotHaveValidationErrorFor(x => x.CategoryId);
        }
    }
}
