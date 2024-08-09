using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Papara.Business.DTOs.Order;
using Papara.Business.Response;
using Papara.Business.Session;
using Papara.Data.Entities;
using Papara.Data.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papara.Business.Commands.CreateOrder
{
    internal class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, ResponseHandler<OrderResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidator<OrderRequest> _validator;
        private readonly ISessionContext _sessionContext;
        private readonly UserManager<User> _userManager;

        public CreateOrderCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IValidator<OrderRequest> validator, ISessionContext sessionContext, UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _validator = validator;
            _sessionContext = sessionContext;
            _userManager = userManager;
        }

        public async Task<ResponseHandler<OrderResponse>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            User user = await _userManager.GetUserAsync(_sessionContext.HttpContext.User);
            if (user == null)
                return new ResponseHandler<OrderResponse>("Login failed.");

            var userResponse = await _userManager.FindByEmailAsync(user.Email);
            if (userResponse == null)
                return new ResponseHandler<OrderResponse>("Login failed.");

            request.Request.UserId = userResponse.Id;

            var validationResult = await _validator.ValidateAsync(request.Request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return new ResponseHandler<OrderResponse>(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
            }

            var order = _mapper.Map<Order>(request.Request);
            order.Code = await GenerateCodeAsync();
            order.OrderDate = DateTime.UtcNow;

            foreach(var detail in order.OrderDetails)
            {
                var product = await _unitOfWork.ProductRepository.GetById(detail.ProductId);
                if (product == null)
                    return new ResponseHandler<OrderResponse>($"Product with ID {detail.ProductId} not found.");

                detail.UnitPrice = product.Price * detail.Quantity;
                order.TotalAmount += detail.UnitPrice;
            }

            await _unitOfWork.OrderRepository.Insert(order);
            await _unitOfWork.CompleteWithTransaction();

            var response = _mapper.Map<OrderResponse>(order);
            return new ResponseHandler<OrderResponse>(response);
        }


        private async Task<int> GenerateCodeAsync()
        {
            int code;
            do
            {
                code = new Random().Next(100000000, 999999999);
            }
            while (await OrderCodeExistsAsync(code));

            return code;
        }

        private async Task<bool> OrderCodeExistsAsync(int code)
        {
            var existingCode = await _unitOfWork.OrderRepository.FirstOrDefault(c => c.Code == code);
            return existingCode != null;
        }
    }
}
