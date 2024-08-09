using Papara.Business.DTOs.Base;
using Papara.Business.DTOs.OrderDetail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Papara.Business.DTOs.Order
{
    public class OrderRequest : BaseRequest
    {
        [JsonIgnore]
        public Guid UserId { get; set; }
        public List<OrderDetailRequest> OrderDetails { get; set; }
    }
}
