using Papara.Business.DTOs.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papara.Business.DTOs.Category
{
    public class CategoryResponse : BaseResponse
    {
        public string Name { get; set; }
        public string Url { get; set; }
    }
}
