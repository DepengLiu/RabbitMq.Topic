using RabbitMQ.Client;

namespace Paodan.RabbitMq.Topic
{
    /// <summary>
    /// 创建MQ消息队列连接
    /// </summary>
    internal class MqConnFactory
    {
        public static IConnection CreateConnection(RabbitMqConfig config)
        {
            ConnectionFactory factory = new ConnectionFactory();
            factory.HostName = config.HostName;
            factory.VirtualHost = config.VirtualHost;
            factory.UserName = config.UserName;
            factory.Password = config.Password;
            factory.AutomaticRecoveryEnabled = true;
            factory.RequestedHeartbeat = 5;
            return factory.CreateConnection();
        }
    }
}
