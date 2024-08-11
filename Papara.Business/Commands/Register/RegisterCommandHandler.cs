using FluentValidation;
using Hangfire;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Papara.Business.DTOs.Authorization;
using Papara.Business.Message;
using Papara.Business.Notification;
using Papara.Business.Response;
using Papara.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papara.Business.Commands.Register
{
    internal class RegisterCommandHandler : IRequestHandler<RegisterCommand, ResponseHandler>
    {
        private readonly UserManager<User> _userManager;
        private readonly IValidator<RegisterRequest> _validator;
        private readonly INotificationService _notificationService;
        private readonly IMessageService _messageService;

        public RegisterCommandHandler(UserManager<User> userManager, IValidator<RegisterRequest> validator, INotificationService notificationService, IMessageService messageService)
        {
            _userManager = userManager;
            _validator = validator;
            _notificationService = notificationService;
            _messageService = messageService;
        }

        public async Task<ResponseHandler> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request.Request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return new ResponseHandler(string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
            }

            var newUser = new User
            {
                UserName = request.Request.Email,
                FirstName = request.Request.FirstName,
                LastName = request.Request.LastName,
                Email = request.Request.Email,
                EmailConfirmed = true,
                TwoFactorEnabled = false,
            };

            var newUserResponse = await _userManager.CreateAsync(newUser, request.Request.Password);
            if (!newUserResponse.Succeeded)
            {
                return new ResponseHandler("Register failed.");
            }

            var addRoleResponse = await _userManager.AddToRoleAsync(newUser, "User");
            if (!addRoleResponse.Succeeded)
            {
                return new ResponseHandler("Register failed.");
            }

            var subject = "Papara | Register";
            var content = $"Hello {request.Request.FirstName} {request.Request.LastName}, You have successfully registered on Papara.";
            var to = request.Request.Email;

            BackgroundJob.Schedule(() =>
                SendEmail(to, subject, content),
                TimeSpan.FromSeconds(30));

            var message = new EmailMessage
            {
                Email = request.Request.Email,
                Subject = subject,
                Body = content
            };
            _messageService.PublishToQueue(message, "emailQueue");

            return new ResponseHandler("Register successfully.");
        }

        [AutomaticRetry(Attempts = 3, DelaysInSeconds = new[] { 10, 15, 18 }, OnAttemptsExceeded = AttemptsExceededAction.Fail)]
        public void SendEmail(string to, string subject, string content)
        {
            _notificationService.SendEmail(to,subject,content).GetAwaiter().GetResult();
        }
    }
}
