using AutoMapper;
using MediatR;
using Papara.Business.DTOs.Coupon;
using Papara.Business.Response;
using Papara.Data.Entities;
using Papara.Data.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papara.Business.Queries.GetAllCoupon
{
    internal class GetAllCouponQueryHandler : IRequestHandler<GetAllCouponsQuery, ResponseHandler<IEnumerable<CouponResponse>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllCouponQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ResponseHandler<IEnumerable<CouponResponse>>> Handle(GetAllCouponsQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<Coupon> couponList = await _unitOfWork.CouponRepository.GetAll();
            var mappedList = _mapper.Map<IEnumerable<CouponResponse>>(couponList);
            var response = new ResponseHandler<IEnumerable<CouponResponse>>(mappedList);

            return response;
        }
    }
}
