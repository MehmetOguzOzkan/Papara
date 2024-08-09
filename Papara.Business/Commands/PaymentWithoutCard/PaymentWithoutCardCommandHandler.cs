using AutoMapper;
using FluentValidation;
using MediatR;
using Papara.Business.DTOs.Order;
using Papara.Business.DTOs.Payment;
using Papara.Business.Response;
using Papara.Data.Entities;
using Papara.Data.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papara.Business.Commands.PaymentWithoutCard
{
    internal class PaymentWithoutCardCommandHandler : IRequestHandler<PaymentWithoutCardCommand, ResponseHandler<OrderResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidator<PaymentRequestWithoutCard> _paymentRequestValidator;

        public PaymentWithoutCardCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IValidator<PaymentRequestWithoutCard> paymentRequestValidator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _paymentRequestValidator = paymentRequestValidator;
        }

        public async Task<ResponseHandler<OrderResponse>> Handle(PaymentWithoutCardCommand request, CancellationToken cancellationToken)
        {
            // 1. Validation
            var validationResult = await _paymentRequestValidator.ValidateAsync(request.Request);
            if (!validationResult.IsValid)
            {
                return new ResponseHandler<OrderResponse>(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
            }

            // 2. Order Check
            var order = await _unitOfWork.OrderRepository.GetById(request.Request.OrderId, nameof(Order.OrderDetails) + "." + nameof(OrderDetail.Product));
            if (order == null)
                return new ResponseHandler<OrderResponse>($"Order not found.");

            if (order.IsPaid)
                return new ResponseHandler<OrderResponse>($"Order is paid.");

            var totalAmount = order.TotalAmount;

            // 3. Coupon Check
            if (request.Request.CouponCode != null && request.Request.CouponCode.Length == 10)
            {
                var coupon = await _unitOfWork.CouponRepository.FirstOrDefault(c => c.Code == request.Request.CouponCode);
                if (coupon == null)
                    return new ResponseHandler<OrderResponse>($"Coupon with Code {request.Request.CouponCode} not found.");

                if (coupon.IsUsed)
                    return new ResponseHandler<OrderResponse>($"Coupon with Code {request.Request.CouponCode} is used.");

                totalAmount -= coupon.DiscountAmount;

                order.CouponAmount = coupon.DiscountAmount;
                order.CouponCode = coupon.Code;
                coupon.IsUsed = true;

                if (totalAmount <= 0)
                {
                    order.OrderDate = DateTime.UtcNow;
                    order.IsPaid = true;
                    var response = _mapper.Map<OrderResponse>(order);

                    return new ResponseHandler<OrderResponse>(response);
                }
                order.UpdateDate = DateTime.UtcNow;
                _unitOfWork.OrderRepository.Update(order);
                _unitOfWork.CouponRepository.Update(coupon);
                await _unitOfWork.CompleteWithTransaction();
            }

            // 4. User Loyalty Points Process
            var user = await _unitOfWork.UserRepository.GetById(order.UserId);
            if (user == null)
                return new ResponseHandler<OrderResponse>($"User not found.");

            if (user.PointsBalance > 0)
            {
                if (totalAmount > user.PointsBalance)
                {
                    totalAmount -= user.PointsBalance;
                    order.PointsUsed = user.PointsBalance;
                    user.PointsBalance = 0;
                }
                else
                {
                    user.PointsBalance -= totalAmount;
                    order.PointsUsed = totalAmount;
                    totalAmount = 0;
                    order.OrderDate = DateTime.UtcNow;
                    order.IsPaid = true;

                    var response = _mapper.Map<OrderResponse>(order);

                    return new ResponseHandler<OrderResponse>(response);
                }
                order.UpdateDate = DateTime.UtcNow;
                _unitOfWork.OrderRepository.Update(order);
                _unitOfWork.UserRepository.Update(user);
                await _unitOfWork.CompleteWithTransaction();
            }

            // 5. User Wallet Transactions
            if (user.WalletBalance < totalAmount)
                return new ResponseHandler<OrderResponse>("Wallet balance is not enough.");

            var discountAmount = order.TotalAmount - totalAmount;
            decimal earnedLoyaltyPoints = 0;
            int rowCount = order.OrderDetails.Count();

            foreach (var detail in order.OrderDetails)
            {
                var product = await _unitOfWork.ProductRepository.GetById(detail.ProductId);
                if (product == null) continue;

                decimal unitPrice = detail.UnitPrice - (discountAmount / rowCount);
                decimal loyaltyPoints = unitPrice * product.LoyaltyPointsRatio;
                var points = Math.Min(loyaltyPoints, (detail.Quantity * product.MaxPoints));
                user.PointsBalance += points;
            }

            _unitOfWork.UserRepository.Update(user);
            await _unitOfWork.CompleteWithTransaction();


            user.WalletBalance -= totalAmount;
            order.UpdateDate = DateTime.UtcNow;
            order.OrderDate = DateTime.UtcNow;
            order.IsPaid = true;

            _unitOfWork.UserRepository.Update(user);
            _unitOfWork.OrderRepository.Update(order);
            await _unitOfWork.CompleteWithTransaction();

            // 6. Response
            var OrderResponse = _mapper.Map<OrderResponse>(order);

            return new ResponseHandler<OrderResponse>(OrderResponse);
        }
    }
}
