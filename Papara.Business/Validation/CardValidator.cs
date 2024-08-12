using FluentValidation;
using Papara.Business.DTOs.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papara.Business.Validation
{
    public class CardValidator : AbstractValidator<Card>
    {
        public CardValidator()
        {
            RuleFor(card => card.CardNumber)
                .Length(16).WithMessage("Card number must be 16 digits long.")
                .Matches("^[0-9]+$").WithMessage("Card number must contain only digits.");

            RuleFor(card => card.Cvc)
                .Length(3).WithMessage("CVC must be 3 digits long.")
                .Matches("^[0-9]+$").WithMessage("CVC must contain only digits.");

            RuleFor(card => card.ExpireMonth)
                .Must(BeAValidMonth).WithMessage("Expire month must be between 01 and 12.");

            RuleFor(card => card.ExpireYear)
                .Length(2).WithMessage("Expire year must be 2 digits long.")
                .Matches("^[0-9]+$").WithMessage("Expire year must contain only digits.");

            RuleFor(card => card)
                .Must(BeAValidExpiryDate).WithMessage("The expiration date cannot be in the past.");
        }

        private bool BeAValidMonth(string expireMonth)
        {
            if (int.TryParse(expireMonth, out int month))
            {
                return month >= 1 && month <= 12;
            }
            return false;
        }

        private bool BeAValidExpiryDate(Card card)
        {
            if (int.TryParse(card.ExpireMonth, out int month) && int.TryParse(card.ExpireYear, out int year))
            {
                year += 2000;
                var expiryDate = new DateTime(year, month, 1).AddMonths(1).AddDays(-1);
                return expiryDate >= DateTime.Now;
            }
            return false;
        }
    }
}
