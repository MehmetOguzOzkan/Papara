using MediatR;
using Papara.Business.DTOs.Order;
using Papara.Business.Response;
using Papara.Data.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papara.Business.Commands.DeleteOrder
{
    internal class DeleteOrderCommandHandler : IRequestHandler<DeleteOrderCommand, ResponseHandler>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteOrderCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseHandler> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _unitOfWork.OrderRepository.GetById(request.Id);
            if (order == null)
                return new ResponseHandler($"Order with ID {request.Id} not found.");

            _unitOfWork.OrderRepository.Delete(order);
            await _unitOfWork.CompleteWithTransaction();

            return new ResponseHandler();
        }
    }
}
