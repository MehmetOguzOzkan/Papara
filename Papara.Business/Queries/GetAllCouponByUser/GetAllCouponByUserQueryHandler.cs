using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Papara.Business.DTOs.Coupon;
using Papara.Business.Response;
using Papara.Business.Session;
using Papara.Data.Entities;
using Papara.Data.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papara.Business.Queries.GetAllCouponByUser
{
    internal class GetAllCouponByUserQueryHandler : IRequestHandler<GetAllCouponByUserQuery, ResponseHandler<IEnumerable<CouponResponse>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly ISessionContext _sessionContext;

        public GetAllCouponByUserQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, UserManager<User> userManager, ISessionContext sessionContext)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
            _sessionContext = sessionContext;
        }

        public async Task<ResponseHandler<IEnumerable<CouponResponse>>> Handle(GetAllCouponByUserQuery request, CancellationToken cancellationToken)
        {
            User user = await _userManager.GetUserAsync(_sessionContext.HttpContext.User);
            if (user == null)
                return new ResponseHandler<IEnumerable<CouponResponse>>("Login failed.");

            IEnumerable<Coupon> couponList = await _unitOfWork.CouponRepository.Where(c => c.UserId == user.Id);
            var mappedList = _mapper.Map<IEnumerable<CouponResponse>>(couponList);
            var response = new ResponseHandler<IEnumerable<CouponResponse>>(mappedList);

            return response;
        }
    }
}
