using AutoMapper;
using MediatR;
using Papara.Business.DTOs.Order;
using Papara.Business.Response;
using Papara.Data.Entities;
using Papara.Data.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papara.Business.Queries.GetOrderById
{
    internal class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, ResponseHandler<OrderResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetOrderByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ResponseHandler<OrderResponse>> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            var order = await _unitOfWork.OrderRepository.GetById(
                request.Id,
                nameof(Order.OrderDetails) + "." + nameof(OrderDetail.Product)
            );

            if (order == null)
                return new ResponseHandler<OrderResponse>($"Order with ID {request.Id} not found.");

            var response = _mapper.Map<OrderResponse>(order);
            return new ResponseHandler<OrderResponse>(response);
        }
    }
}
