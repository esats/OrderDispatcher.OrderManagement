using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System.Text;

namespace OrderDispatcher.OrderManagement.API.Infrastructure
{
    public class OrderMessagePublisher
    {
        private readonly IConfiguration _configuration;

        public OrderMessagePublisher(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public bool PublishOrderId(long orderId, string routingKey, out string error)
        {
            error = string.Empty;

            var url = _configuration["RabbitMQ:Url"];
            var exchange = _configuration["RabbitMQ:Exchange"];
            var queue = _configuration["RabbitMQ:Queue"];

            if (string.IsNullOrWhiteSpace(url) ||
                string.IsNullOrWhiteSpace(exchange) ||
                string.IsNullOrWhiteSpace(queue) ||
                string.IsNullOrWhiteSpace(routingKey))
            {
                error = "RabbitMQ configuration is missing.";
                return false;
            }

            try
            {
                var factory = new ConnectionFactory
                {
                    Uri = new Uri(url)
                };

                using var connection = factory.CreateConnection();
                using var channel = connection.CreateModel();

                channel.ExchangeDeclare(exchange: exchange, type: ExchangeType.Topic, durable: true, autoDelete: false);
                channel.QueueDeclare(queue: queue, durable: true, exclusive: false, autoDelete: false);
                channel.QueueBind(queue: queue, exchange: exchange, routingKey: routingKey);

                var body = Encoding.UTF8.GetBytes(orderId.ToString());
                var props = channel.CreateBasicProperties();
                props.Persistent = true;
                props.ContentType = "text/plain";

                channel.BasicPublish(
                    exchange: exchange,
                    routingKey: routingKey,
                    basicProperties: props,
                    body: body);

                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }
    }
}
