using Papara.Business.DTOs.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papara.Business.DTOs.OrderDetail
{
    public class OrderDetailResponse : BaseResponse
    {
        public OrderDetailProductResponse Product { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
