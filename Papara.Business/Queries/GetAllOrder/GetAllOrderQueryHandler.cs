using AutoMapper;
using MediatR;
using Papara.Business.DTOs.Order;
using Papara.Business.DTOs.OrderDetail;
using Papara.Business.Response;
using Papara.Data.Entities;
using Papara.Data.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papara.Business.Queries.GetAllOrder
{
    internal class GetAllOrderQueryHandler : IRequestHandler<GetAllOrderQuery, ResponseHandler<IEnumerable<OrderResponse>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllOrderQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ResponseHandler<IEnumerable<OrderResponse>>> Handle(GetAllOrderQuery request, CancellationToken cancellationToken)
        {
            var orders = await _unitOfWork.OrderRepository.GetAll(nameof(Order.User), nameof(Order.OrderDetails) + "." + nameof(OrderDetail.Product));

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
