using AutoMapper;
using FluentValidation;
using MediatR;
using Papara.Business.DTOs.Coupon;
using Papara.Business.Response;
using Papara.Data.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papara.Business.Commands.UpdateCoupon
{
    internal class UpdateCouponCommandHandler : IRequestHandler<UpdateCouponCommand, ResponseHandler<CouponResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidator<CouponRequest> _validator;

        public UpdateCouponCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IValidator<CouponRequest> validator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<ResponseHandler<CouponResponse>> Handle(UpdateCouponCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request.Request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return new ResponseHandler<CouponResponse>(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
            }

            var coupon = await _unitOfWork.CouponRepository.GetById(request.Id);
            if (coupon == null)
                return new ResponseHandler<CouponResponse>("Coupon not found.");

            _mapper.Map(request.Request, coupon);
            _unitOfWork.CouponRepository.Update(coupon);
            await _unitOfWork.CompleteWithTransaction();

            var updatedCoupon = await _unitOfWork.CouponRepository.GetById(coupon.Id);
            var response = _mapper.Map<CouponResponse>(updatedCoupon);
            return new ResponseHandler<CouponResponse>(response);
        }
    }
}
