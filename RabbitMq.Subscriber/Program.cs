using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;

namespace RabbitMq.Subscriber
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqps://kfsdoqic:jn0RezxxesWFhHedSXKfrxlcieC0PsC_@fish.rmq.cloudamqp.com/kfsdoqic");
            using var connection = factory.CreateConnection();

            var channel = connection.CreateModel();
   

            //kuyruk oluşturma işlemi --isimleri aynı olmamalı 3 instance oluşturursam üçü de aynı instancea bağlanır 
            var randomQueueName = channel.QueueDeclare().QueueName; // random kuyruk oluşturma propertysi 

            //subscribe ilgili exchange e bind işlemi bittikten sonra kuyruğu silsin diye kuyruk declare etmiyoruz bind ediyoruz
            channel.QueueBind(randomQueueName,"logs-fanout","",null);



            channel.BasicQos(0, 1, false);
            var subscriber = new EventingBasicConsumer(channel);

            channel.BasicConsume(randomQueueName, false, subscriber);
            Console.WriteLine("Loglar dinleniyor .." );


            subscriber.Received += (object sender, BasicDeliverEventArgs e) =>
            {
                var message = Encoding.UTF8.GetString(e.Body.ToArray());

                Thread.Sleep(1500); 
                Console.WriteLine("Gelen mesaj : " + message);

                channel.BasicAck(e.DeliveryTag, false); 
            };

            Console.ReadLine();
        }


    }
}