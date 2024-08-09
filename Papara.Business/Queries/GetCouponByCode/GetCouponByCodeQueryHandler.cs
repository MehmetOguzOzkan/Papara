using AutoMapper;
using MediatR;
using Papara.Business.DTOs.Coupon;
using Papara.Business.Response;
using Papara.Data.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papara.Business.Queries.GetCouponByCode
{
    internal class GetCouponByCodeQueryHandler : IRequestHandler<GetCouponByCodeQuery, ResponseHandler<CouponResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetCouponByCodeQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ResponseHandler<CouponResponse>> Handle(GetCouponByCodeQuery request, CancellationToken cancellationToken)
        {
            var coupon = await _unitOfWork.CouponRepository.FirstOrDefault(c => c.Code == request.Code);
            if (coupon == null)
            {
                return new ResponseHandler<CouponResponse>("Coupon not found");
            }

            var response = _mapper.Map<CouponResponse>(coupon);
            return new ResponseHandler<CouponResponse>(response);
        }
    }
}
