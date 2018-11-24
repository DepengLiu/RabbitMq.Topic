namespace Paodan.RabbitMq.Topic
{
    /// <summary>
    /// 生产者工厂类
    /// </summary>
    public class ProducerFactory
    {
        /// <summary>
        /// 获取此工厂的MQ相关配置
        /// </summary>
        public RabbitMqConfig Config { get; private set; }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="config">MQ相关配置</param>
        public ProducerFactory(RabbitMqConfig config)
        {
            Config = config;
        }
        /// <summary>
        /// 创建生产者实例
        /// </summary>
        /// <param name="producerId"></param>
        /// <returns></returns>
        public Producer.ITopicProducer CreateProducer(string producerId)
        {
            return new Producer.TopicProducer(producerId, Config);
        }
    }
}
