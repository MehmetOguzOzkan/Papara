using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Papara.Business.Commands.CreateCategory;
using Papara.Business.DTOs.Category;
using Papara.Business.DTOs.Coupon;
using Papara.Business.Response;
using Papara.Data.Entities;
using Papara.Data.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Papara.Business.Commands.CreateCoupon
{
    public class CreateCouponCommandHandler : IRequestHandler<CreateCouponCommand, ResponseHandler<CouponResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidator<CouponRequest> _validator;

        public CreateCouponCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IValidator<CouponRequest> validator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<ResponseHandler<CouponResponse>> Handle(CreateCouponCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request.Request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return new ResponseHandler<CouponResponse>(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
            }

            var mapped = _mapper.Map<Coupon>(request.Request);
            mapped.Code = GenerateCode(10);

            await _unitOfWork.CouponRepository.Insert(mapped);
            await _unitOfWork.CompleteWithTransaction();

            var response = _mapper.Map<CouponResponse>(mapped);
            return new ResponseHandler<CouponResponse>(response);
        }

        private async Task<string> GenerateUniqueCodeAsync()
        {
            string code;
            do
            {
                code = GenerateCode(10);
            }
            while (await CouponCodeExistsAsync(code));

            return code;
        }

        private async Task<bool> CouponCodeExistsAsync(string code)
        {
            var existingCoupon = await _unitOfWork.CouponRepository.FirstOrDefault(c => c.Code == code);
            return existingCoupon != null;
        }


        private string GenerateCode(int length)
        {
            var random = new Random();
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
