namespace Paodan.RabbitMq.Topic
{
    /// <summary>
    /// 消费者工厂类
    /// </summary>
    public class ConsumerFactory
    {
        /// <summary>
        /// 获取此工厂的MQ相关配置
        /// </summary>
        public RabbitMqConfig Config { get; private set; }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="config">MQ相关配置</param>
        public ConsumerFactory(RabbitMqConfig config)
        {
            Config = config;
        }
        /// <summary>
        /// 创建消费者实例
        /// </summary>
        /// <param name="consumerId">消费者id  每个消费者会创建一个名为consumerId的队列，，并绑定到指定的Exchange中，同ConsumerId的消费者 平摊消息</param>
        /// <param name="durable">是否支持持久化 如果要支持离线重连后继续消费离线期间的消息，必须要支持持久化</param>
        /// <returns></returns>
        public Consumer.ITopicConsumer CreateConsumer(string consumerId, bool durable)
        {
            return new Consumer.TopicConsumer(consumerId, Config, durable);
        }
    }
}
