using FluentValidation;
using Papara.Business.DTOs.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papara.Business.Validation
{
    public class ProductRequestValidator : AbstractValidator<ProductRequest>
    {
        public ProductRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Product name is required.")
                .MaximumLength(200).WithMessage("Product name must not exceed 200 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than zero.");

            RuleFor(x => x.LoyaltyPointsRatio)
                .InclusiveBetween(0, 1).WithMessage("LoyaltyPointsRatio must be between 0 and 1.");

            RuleFor(x => x.MaxPoints)
                .GreaterThan(0).WithMessage("MaxPoints must be greater than zero.");

            RuleFor(x => x.CategoryIds)
                .NotEmpty().WithMessage("At least one category is required.");
        }
    }
}
