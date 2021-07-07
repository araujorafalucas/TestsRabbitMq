using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace TestsRabbitMq.Consumer
{
    class Program
    {
        static IConnection connection;
        static IModel channel;
        static EventingBasicConsumer consumer;

        static void Main(string[] args)
        {
            Console.WriteLine("Iniciando conexão do consumer...");

            if (!CreateConnection())
            {
                Console.WriteLine("Fechando programa, aperte qualquer tecla...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Digite qualquer tecla para sair...");
            Console.ReadKey();
            
            channel.Close();            
            connection.Close();
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

                consumer = new EventingBasicConsumer(channel);
                consumer.Received += Consumer_Received;

                channel.BasicConsume(queue: "queue.test", autoAck: false, consumer: consumer);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro : " + ex.Message);
                return false;
            }

        }

        private static void Consumer_Received(object sender, BasicDeliverEventArgs e)
        {
            try
            {
                var body = e.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                channel.BasicAck(e.DeliveryTag, false);

                Console.WriteLine("Mensagem Recebida: " + message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro: " + ex.Message);
            }
        
        }
    }
}
