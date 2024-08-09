using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papara.Data.Entities
{
    public class Category : BaseEntity
    {
        public string Name { get; set; }
        public string Url { get; set; }

        public virtual ICollection<ProductCategory> ProductCategories { get; set; }
    }
}
