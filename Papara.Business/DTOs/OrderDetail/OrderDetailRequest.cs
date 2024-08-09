using Papara.Business.DTOs.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papara.Business.DTOs.OrderDetail
{
    public class OrderDetailRequest : BaseRequest
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
