using Papara.Business.DTOs.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papara.Business.DTOs.Payment
{
    public class PaymentResponse
    {
        public Card Card { get; set; }
        public OrderResponse Order { get; set; }
    }
}
