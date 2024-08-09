using MediatR;
using Papara.Business.DTOs.Coupon;
using Papara.Business.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papara.Business.Commands.CreateCoupon
{
    public record CreateCouponCommand(CouponRequest Request) : IRequest<ResponseHandler<CouponResponse>>;
}
