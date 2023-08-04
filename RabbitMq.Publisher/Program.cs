using RabbitMQ.Client;
using System;
using System.Linq;
using System.Text;

namespace RabbitMq.Publisher
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqps://kfsdoqic:jn0RezxxesWFhHedSXKfrxlcieC0PsC_@fish.rmq.cloudamqp.com/kfsdoqic");

            using var connection = factory.CreateConnection();

            var channel = connection.CreateModel();

            // channel.QueueDeclare("hello-queue", true, false, false); => kuyruğu artık subscribe oluşturacak 
            //durable :true => kuyruklar hiç bi zaman kaybolmaz. fiziksel olarak tutulur. 

            channel.ExchangeDeclare("logs-fanout",type: ExchangeType.Fanout,durable:true);  //=>Exchange oluşturma işlemi

            Enumerable.Range(1, 50).ToList().ForEach(x =>
            {
                string message = $"log {x} ";
                var messageBody = Encoding.UTF8.GetBytes(message);


                //channel.BasicPublish(string.Empty, "hello-queue", null, messageBody); => eskisi
                channel.BasicPublish("logs-fanout", "", null, messageBody);  //eskisinde direkt kuyruğa gönderdiğimiz içi  kuyruğun ismini vermiştik burdda boş kalacak.

                Console.WriteLine($"Mesaj gönderilmiştir.: {message}");
            }
            );

            Console.ReadLine();
        }
    }
}
