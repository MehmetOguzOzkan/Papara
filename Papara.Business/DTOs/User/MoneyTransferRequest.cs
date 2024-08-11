using Papara.Business.DTOs.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papara.Business.DTOs.User
{
    public class MoneyTransferRequest
    {
        public Card Card { get; set; }
        public decimal Amount { get; set; }
    }
}
