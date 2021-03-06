using RabbitMQ.Client;
using System;
using System.Text;

namespace TestsRabbitMq.Producer
{
    class Program
    {
        static IConnection connection;
        static IModel channel;

        static void Main(string[] args)
        {            
            Console.WriteLine("Iniciando conexão do Producer...");

            if (!CreateConnection())
            {
                Console.WriteLine("Fechando programa, aperte qualquer tecla...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Digite uma mensagem:");

            var input = Console.ReadLine();

            while (input != "quit")
            {
                SendMessage(input);

                Console.WriteLine("Mensagem enviada! Digite outra mensagem:");                
                input = Console.ReadLine();
            }

        }

        private static bool CreateConnection()
        {
            try
            {

                var factory = new ConnectionFactory()
                {
                    VirtualHost = "/",
                    HostName = "localhost",
                    UserName = "guest",
                    Password = "guest"
                };

                connection = factory.CreateConnection();
                channel = connection.CreateModel();

                channel.ExchangeDeclare("exchange.test", ExchangeType.Fanout, true, false);
                channel.QueueDeclare(queue: "queue.test", durable: true, exclusive: false, autoDelete: false, arguments: null);
                channel.QueueBind("queue.test", "exchange.test", "", null);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro : " + ex.Message);
                return false;
            }

        }

        private static void SendMessage(string message)
        {
            try
            {
                var body = Encoding.UTF8.GetBytes(message);

                var props = channel.CreateBasicProperties();
                props.DeliveryMode = 2;

                channel.BasicPublish(exchange: "exchange.test",
                                     routingKey: "",
                                     basicProperties: props,
                                     body: body);
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
