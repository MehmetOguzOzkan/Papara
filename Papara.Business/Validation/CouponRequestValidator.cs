using FluentValidation;
using Papara.Business.DTOs.Coupon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papara.Business.Validation
{
    internal class CouponRequestValidator : AbstractValidator<CouponRequest>
    {
        public CouponRequestValidator()
        {
            RuleFor(x => x.DiscountAmount)
                .GreaterThan(0).WithMessage("Discount amount must be greater than zero.");

            RuleFor(x => x.ValidFrom)
                .LessThan(x => x.ValidTo).WithMessage("ValidFrom must be earlier than ValidTo.");

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required.");
        }
    }
}
