using Papara.Business.DTOs.Base;
using Papara.Business.DTOs.OrderDetail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papara.Business.DTOs.Order
{
    public class OrderResponse : BaseResponse
    {
        public Guid UserId { get; set; }
        public int Code { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal CouponAmount { get; set; }
        public string CouponCode { get; set; }
        public decimal PointsUsed { get; set; }
        public DateTime OrderDate { get; set; }
        public List<OrderDetailResponse> OrderDetails { get; set; }
    }
}
