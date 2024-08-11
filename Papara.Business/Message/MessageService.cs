using Microsoft.AspNetCore.Connections;
using Newtonsoft.Json;
using Papara.Business.Notification;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Papara.Business.Message
{
    internal class MessageService : IMessageService
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public MessageService()
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                Port = 5672,
                UserName = "guest",
                Password = "guest"
            };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(queue: "emailQueue",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);
        }

        public async Task PublishToQueue(EmailMessage message, string queueName)
        {
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

            _channel.BasicPublish(
                exchange: "",
                routingKey: queueName,
                basicProperties: null,
                body: body);
        }

        public void Dispose()
        {
            _channel.Close();
            _connection.Close();
        }

    }
}
