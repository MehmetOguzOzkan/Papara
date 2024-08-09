using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papara.Data.Entities
{
    public class Order : BaseEntity
    {
        public Guid UserId { get; set; }
        public virtual User User { get; set; }

        public int Code { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal? CouponAmount { get; set; }
        public string? CouponCode { get; set; }
        public decimal PointsUsed { get; set; }
        public bool IsPaid { get; set; }
        public DateTime OrderDate { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
