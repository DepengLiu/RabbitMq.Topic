using Paodan.RabbitMq.Topic;
using System;

namespace Producer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Producer";
            RabbitMqConfig config = new RabbitMqConfig();
            config.ExchangeName = "test";
            config.HostName = "localhost";
            config.Password = "guest";
            config.UserName = "guest";
            config.VirtualHost = "/";

            ProducerFactory factory = new ProducerFactory(config);
            var producer = factory.CreateProducer("Producer1");
            Console.WriteLine("准备就绪");
            Console.WriteLine("请输入要发送的消息,格式: Topic,Message。按 Q 退出");
            while (true)
            {

                var input = Console.ReadLine();
                if ("Q".Equals(input, StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }
                var array = input.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (array.Length != 2)
                {
                    Console.WriteLine("消息格式错误,请重新输入");
                    continue;
                }
                producer.Publish(array[0], Guid.NewGuid().ToString("N"), array[1]);
            }
        }
    }
}
