using FluentValidation;
using Papara.Business.DTOs.Payment;
using Papara.Business.DTOs.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papara.Business.Validation
{
    public class MoneyTransferRequestValidator : AbstractValidator<MoneyTransferRequest>
    {
        public MoneyTransferRequestValidator() 
        {
            RuleFor(request => request.Card)
                .NotNull().WithMessage("Card information is required.")
                .SetValidator(new CardValidator());

            RuleFor(request => request.Amount)
                .GreaterThan(0).WithMessage("Amount must be greater than zero.");
        }
    }
}
