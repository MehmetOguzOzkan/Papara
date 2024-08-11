using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Papara.Business.Notification
{
    internal class NotificationService : INotificationService
    {
        private readonly SmtpSettings _smtpSettings;

        public NotificationService(IOptions<SmtpSettings> smtpSettings)
        {
            _smtpSettings = smtpSettings.Value;
        }

        public async Task SendEmail(string to, string subject, string body)
        {
            var fromAddress = new MailAddress(_smtpSettings.FromAddress, _smtpSettings.FromName);
            var toAddress = new MailAddress(to);
            var smtpClient = new SmtpClient
            {
                Host = _smtpSettings.Host,
                Port = _smtpSettings.Port,
                EnableSsl = _smtpSettings.EnableSsl,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_smtpSettings.FromAddress, _smtpSettings.FromPassword)
            };

            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                await smtpClient.SendMailAsync(message);
            }
        }
    }
}
