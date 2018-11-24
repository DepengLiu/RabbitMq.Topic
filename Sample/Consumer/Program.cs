using Paodan.RabbitMq.Topic;
using Paodan.RabbitMq.Topic.Consumer;
using System;

namespace Consumer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Consumer";
            RabbitMqConfig config = new RabbitMqConfig();

            config.ExchangeName = "test";
            config.HostName = "localhost";
            config.Password = "guest";
            config.UserName = "guest";
            config.VirtualHost = "/";

            ConsumerFactory factory = new ConsumerFactory(config);
            var consumer = factory.CreateConsumer("ConsumerId1", true);

            consumer.MessageReceive += (sender, e) =>
            {
                Console.WriteLine($"{sender.ConsumerId}收到消息,Topic:{e.Topic}:Key:{e.Key},Content:{e.Content}");
                e.Result = ConsumeResult.Success;
            };

            Console.WriteLine("请输入要订阅的Topic");
            var input = Console.ReadLine();
            consumer.Subscribe(input);

            Console.ReadKey();
        }

        //private static void Consumer_MessageReceive(ITopicConsumer sender, MessageReceiveEventArgs e)
        //{

        //}
    }
}
