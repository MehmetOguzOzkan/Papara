using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papara.Business.DTOs.ProductCategory
{
    public class ProductCategoryRequest
    {
        public Guid ProductId { get; set; }
        public Guid CategoryId { get; set; }
    }
}
