using Papara.Business.Notification;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Papara.Business.Message;
using Newtonsoft.Json;

namespace Papara.Business.Job
{
    public class EmailProcessorJob
    {
        private readonly INotificationService _notificationService;
        private readonly IMessageService _messageService;

        public EmailProcessorJob(INotificationService notificationService, IMessageService messageService)
        {
            _notificationService = notificationService;
            _messageService = messageService;
        }

        public void ProcessEmailQueue()
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest"
            };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var emailMessage = JsonConvert.DeserializeObject<EmailMessage>(message);

                    _notificationService.SendEmail(emailMessage.Email, emailMessage.Subject, emailMessage.Body).Wait();
                };

                channel.BasicConsume(queue: "emailQueue", autoAck: true, consumer: consumer);
            }
        }
    }
}
