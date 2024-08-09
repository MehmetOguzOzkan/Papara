using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papara.Data.Entities
{
    public class Product : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal LoyaltyPointsRatio { get; set; }
        public int MaxPoints { get; set; }
        public bool InStock { get; set; }

        public virtual ICollection<ProductCategory> ProductCategories { get; set; }
    }
}
