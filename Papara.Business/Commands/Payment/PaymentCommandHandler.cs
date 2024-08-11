using AutoMapper;
using FluentValidation;
using Hangfire;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Papara.Business.DTOs.Order;
using Papara.Business.DTOs.Payment;
using Papara.Business.Message;
using Papara.Business.Notification;
using Papara.Business.Response;
using Papara.Business.Session;
using Papara.Data.Entities;
using Papara.Data.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papara.Business.Commands.Payment
{
    internal class PaymentCommandHandler : IRequestHandler<PaymentCommand, ResponseHandler<PaymentResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidator<PaymentRequest> _paymentRequestValidator;
        private readonly IMessageService _messageService;
        private readonly INotificationService _notificationService;

        public PaymentCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IValidator<PaymentRequest> paymentRequestValidator, IMessageService messageService, INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _paymentRequestValidator = paymentRequestValidator;
            _messageService = messageService;
            _notificationService = notificationService;
        }

        public async Task<ResponseHandler<PaymentResponse>> Handle(PaymentCommand request, CancellationToken cancellationToken)
        {
            // 1. Validation
            var validationResult = await _paymentRequestValidator.ValidateAsync(request.Request);
            if (!validationResult.IsValid)
            {
                return new ResponseHandler<PaymentResponse>(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
            }

            // 2. Order Check
            var order = await _unitOfWork.OrderRepository.GetById(request.Request.OrderId, nameof(Order.OrderDetails) + "." + nameof(OrderDetail.Product));
            if (order == null)
                return new ResponseHandler<PaymentResponse>($"Order not found.");

            if(order.IsPaid)
                return new ResponseHandler<PaymentResponse>($"Order is paid.");

            var user = await _unitOfWork.UserRepository.GetById(order.UserId);
            if (user == null)
                return new ResponseHandler<PaymentResponse>($"User not found.");

            var subject = "Papara | Payment Order";
            var content = $"Hello {user.FirstName} {user.LastName}, You have successfully ordered on Papara.";
            var to = user.Email;

            var message = new EmailMessage
            {
                Email = user.Email,
                Subject = subject,
                Body = content
            };

            var totalAmount = order.TotalAmount;

            // 3. Coupon Check
            if (request.Request.CouponCode != null && request.Request.CouponCode.Length == 10)
            {
                var coupon = await _unitOfWork.CouponRepository.FirstOrDefault(c => c.Code == request.Request.CouponCode);
                if (coupon == null)
                    return new ResponseHandler<PaymentResponse>($"Coupon with Code {request.Request.CouponCode} not found.");

                if(coupon.IsUsed)
                    return new ResponseHandler<PaymentResponse>($"Coupon with Code {request.Request.CouponCode} is used.");

                totalAmount -= coupon.DiscountAmount;

                order.CouponAmount = coupon.DiscountAmount;
                order.CouponCode = coupon.Code;
                coupon.IsUsed = true;

                if (totalAmount <= 0)
                {
                    order.OrderDate = DateTime.UtcNow;
                    order.IsPaid = true;
                    var response = new PaymentResponse
                    {
                        Card = request.Request.Card,
                        Order = _mapper.Map<OrderResponse>(order)
                    };

                    BackgroundJob.Schedule(() =>
                        SendEmail(to, subject, content),
                        TimeSpan.FromSeconds(30));

                    _messageService.PublishToQueue(message, "emailQueue");

                    return new ResponseHandler<PaymentResponse>(response);
                }
                order.UpdateDate = DateTime.UtcNow;
                _unitOfWork.OrderRepository.Update(order);
                _unitOfWork.CouponRepository.Update(coupon);
                await _unitOfWork.CompleteWithTransaction();
            }

            // 4. User Loyalty Points Process
            if(user.PointsBalance > 0)
            {
                if(totalAmount > user.PointsBalance)
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

                    var response = new PaymentResponse
                    {
                        Card = request.Request.Card,
                        Order = _mapper.Map<OrderResponse>(order)
                    };

                    BackgroundJob.Schedule(() =>
                        SendEmail(to, subject, content),
                        TimeSpan.FromSeconds(30));

                    _messageService.PublishToQueue(message, "emailQueue");

                    return new ResponseHandler<PaymentResponse>(response);
                }
                order.UpdateDate = DateTime.UtcNow;
                _unitOfWork.OrderRepository.Update(order);
                _unitOfWork.UserRepository.Update(user);
                await _unitOfWork.CompleteWithTransaction();
            }

            // 5. Card Payment Transactions
            if (request.Request.Card.Balance < totalAmount)
                return new ResponseHandler<PaymentResponse>("Card balance is not enough.");

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


            request.Request.Card.Balance -= totalAmount;
            order.UpdateDate = DateTime.UtcNow;
            order.OrderDate = DateTime.UtcNow;
            order.IsPaid = true;

            _unitOfWork.OrderRepository.Update(order);
            await _unitOfWork.CompleteWithTransaction();

            // 6. Response
            var paymentResponse = new PaymentResponse
            {
                Card = request.Request.Card,
                Order = _mapper.Map<OrderResponse>(order)
            };

            BackgroundJob.Schedule(() =>
                SendEmail(to, subject, content),
                TimeSpan.FromSeconds(30));

            _messageService.PublishToQueue(message, "emailQueue");

            return new ResponseHandler<PaymentResponse>(paymentResponse);
        }

        [AutomaticRetry(Attempts = 3, DelaysInSeconds = new[] { 10, 15, 18 }, OnAttemptsExceeded = AttemptsExceededAction.Fail)]
        public void SendEmail(string to, string subject, string content)
        {
            _notificationService.SendEmail(to, subject, content).GetAwaiter().GetResult();
        }
    }
}
