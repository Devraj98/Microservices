using PlatformService.Dtos;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace platformservice.AsyncDataServices
{
    public class MessageBusClient : IMessageBusClient
    {
        private readonly IConfiguration _config;
        private readonly IConnection? _connection;
        private readonly IModel? _channel;

        public MessageBusClient(IConfiguration config)
        {
            
            _config = config;
            var factory = new ConnectionFactory() { HostName = _config["RabbitMQHost"], Port = int.Parse(_config["RabbitMQPort"] ?? "") };
            try
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();
                _channel.ExchangeDeclare(exchange : "trigger", type: ExchangeType.Fanout);
                //_channel.QueueDeclare(queue: "DurableQueue",
                //    durable: true,
                //    autoDelete: false,
                //    arguments: null);
                _connection.ConnectionShutdown += RabbitMQ_Shutdown;

                Console.WriteLine("--> Connected to Message Queue ");
            }
            
            catch(Exception ex) 
            {
                Console.WriteLine($"--> Connecting to RabbitMQ Bus : {ex.Message}");
            }
        }

        public void PublishNewPlatform(PlatformPublishedDto platformPublishedDto)
        {
            var message = JsonSerializer.Serialize(platformPublishedDto);
            if (_connection.IsOpen)
            {
                Console.WriteLine("--> RabbitMQ connection open , Semding Message ");
                SendMessage(message);
            }
            else
            {
                Console.WriteLine("--> RabbitMQ connection closed , sending Message failed ");
            }
        }


        private void SendMessage(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(exchange: "trigger",
                                  routingKey: "",
                                  basicProperties: null,
                                  body: body);
            Console.WriteLine($"--> We have sent message : {message}");
        }


        private void RabbitMQ_Shutdown(object sender, ShutdownEventArgs args)
        {
            Console.WriteLine("--> RabbitMQ connection shutdown");
        }

        public void Dispose()
        {
            Console.WriteLine("MessageBus disposed");
            if (_channel.IsOpen)
            {
                _channel.Close();
                _connection?.Close();
            }
        }
    }
}
