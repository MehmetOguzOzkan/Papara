using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papara.Business.DTOs.Payment
{
    public class Card
    {
        public string CardNumber { get; set; }
        public string Cvc { get; set; }
        public string ExpireMonth { get; set; }
        public string ExpireYear { get; set; }
        public decimal Balance { get; set; }
    }
}
