using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitMQ.Receiver
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost", UserName = "guest", Password = "guest" };
            using (IConnection connection = factory.CreateConnection())
            {
                var mainQueue = "RabbitMQ_" + "Animals";
                var mainQueue2 = "RabbitMQ_" + "Animals" + "_2";
                var mainQueue3 = "RabbitMQ_" + "Animals" + "_3";

                IModel mainQueueChannel = connection.CreateModel();

                var mainQueueConsumer = new EventingBasicConsumer(mainQueueChannel);
                mainQueueConsumer.Received += async (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var animalName = Encoding.UTF8.GetString(body);
                    Console.WriteLine(animalName);
                    mainQueueChannel.BasicAck(ea.DeliveryTag, false);
                };
                mainQueueChannel.BasicConsume(queue: mainQueue, consumer: mainQueueConsumer);
                mainQueueChannel.BasicConsume(queue: mainQueue2, consumer: mainQueueConsumer);
                mainQueueChannel.BasicConsume(queue: mainQueue3, consumer: mainQueueConsumer);
                Console.ReadLine();
            }
        }
    }
}
