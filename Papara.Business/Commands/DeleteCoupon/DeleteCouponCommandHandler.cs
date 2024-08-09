using MediatR;
using Papara.Business.Response;
using Papara.Data.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papara.Business.Commands.DeleteCoupon
{
    internal class DeleteCouponCommandHandler : IRequestHandler<DeleteCouponCommand, ResponseHandler>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteCouponCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseHandler> Handle(DeleteCouponCommand request, CancellationToken cancellationToken)
        {
            var coupon = await _unitOfWork.CouponRepository.GetById(request.Id);
            if (coupon == null)
                return new ResponseHandler("Coupon not found.");

            _unitOfWork.CouponRepository.SoftDelete(coupon);
            await _unitOfWork.CompleteWithTransaction();

            return new ResponseHandler("Coupon deleted successfully.");
        }
    }
}
