using MediatR;
using Papara.Business.DTOs.Coupon;
using Papara.Business.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papara.Business.Commands.UpdateCoupon
{
    public record UpdateCouponCommand(Guid Id, CouponRequest Request) : IRequest<ResponseHandler<CouponResponse>>;

}
