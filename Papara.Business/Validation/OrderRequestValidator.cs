using FluentValidation;
using Papara.Business.DTOs.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papara.Business.Validation
{
    public class OrderRequestValidator : AbstractValidator<OrderRequest>
    {
        public OrderRequestValidator()
        {
            RuleForEach(x => x.OrderDetails)
                .SetValidator(new OrderDetailRequestValidator());
        }
    }
}
