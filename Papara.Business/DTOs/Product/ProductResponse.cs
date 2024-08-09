using Papara.Business.DTOs.Base;
using Papara.Business.DTOs.Category;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papara.Business.DTOs.Product
{
    public class ProductResponse : BaseResponse
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal LoyaltyPointsRatio { get; set; }
        public int MaxPoints { get; set; }
        public bool InStock { get; set; }
        public IEnumerable<CategoryResponse> Categories { get; set; }
    }
}
