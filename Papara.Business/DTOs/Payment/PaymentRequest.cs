using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papara.Business.DTOs.Payment
{
    public class PaymentRequest
    {
        public Card Card { get; set; }
        public Guid OrderId { get; set; }
        public string? CouponCode { get; set; }
    }
}
