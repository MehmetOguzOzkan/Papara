using Papara.Business.DTOs.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papara.Business.DTOs.Category
{
    public class CategoryRequest : BaseRequest
    {
        public string Name { get; set; }
    }
}
