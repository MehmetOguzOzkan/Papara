using FluentValidation;
using Papara.Business.DTOs.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papara.Business.Validation
{
    internal class PaymentRequestValidator : AbstractValidator<PaymentRequest>
    {
        public PaymentRequestValidator()
        {
            RuleFor(request => request.Card)
                .NotNull().WithMessage("Card information is required.")
                .SetValidator(new CardValidator()); // Use the CardValidator for Card property

            RuleFor(request => request.OrderId)
                .NotEmpty().WithMessage("Order ID is required.");

            RuleFor(request => request.CouponCode)
                .Matches(@"^[A-Z0-9]{0,10}$").WithMessage("Coupon code, if provided, must be up to 10 alphanumeric characters.");
        }
    }
}
