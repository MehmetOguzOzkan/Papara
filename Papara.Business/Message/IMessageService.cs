using Papara.Business.Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Papara.Business.Message
{
    public interface IMessageService
    {
        Task PublishToQueue(EmailMessage message, string queueName);
    }
}
