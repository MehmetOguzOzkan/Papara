using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papara.Data.Entities
{
    public class User : IdentityUser<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public decimal PointsBalance { get; set; }
        public decimal WalletBalance { get; set; }
        public string WalletCurrency { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<Coupon> Coupons { get; set; }
    }
}
