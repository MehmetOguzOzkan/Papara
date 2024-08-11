using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Papara.Business.DTOs.Coupon;
using Papara.Business.DTOs.Order;
using Papara.Business.DTOs.OrderDetail;
using Papara.Business.Response;
using Papara.Business.Session;
using Papara.Data.Entities;
using Papara.Data.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papara.Business.Queries.GetAllOrderByUser
{
    internal class GetAllOrderByUserQueryHandler : IRequestHandler<GetAllOrderByUserQuery, ResponseHandler<IEnumerable<OrderResponse>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly ISessionContext _sessionContext;

        public GetAllOrderByUserQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, UserManager<User> userManager, ISessionContext sessionContext)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
            _sessionContext = sessionContext;
        }

        public async Task<ResponseHandler<IEnumerable<OrderResponse>>> Handle(GetAllOrderByUserQuery request, CancellationToken cancellationToken)
        {
            User user = await _userManager.GetUserAsync(_sessionContext.HttpContext.User);
            if (user == null)
                return new ResponseHandler<IEnumerable<OrderResponse>>("Login failed.");

            var orders = await _unitOfWork.OrderRepository.Where(o => o.UserId == user.Id, nameof(Order.User), nameof(Order.OrderDetails) + "." + nameof(OrderDetail.Product));
            
            var orderResponses = orders.Select(order =>
            {
                var orderResponse = _mapper.Map<OrderResponse>(order);
                orderResponse.OrderDetails = order.OrderDetails.Select(detail =>
                {
                    var detailResponse = _mapper.Map<OrderDetailResponse>(detail);
                    return detailResponse;
                }).ToList();

                return orderResponse;
            }).ToList();

            return new ResponseHandler<IEnumerable<OrderResponse>>(orderResponses);
        }
    }
}
