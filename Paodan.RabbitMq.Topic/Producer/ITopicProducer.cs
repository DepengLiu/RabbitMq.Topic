namespace Paodan.RabbitMq.Topic.Producer
{
    /// <summary>
    /// 基于Topic的生产者接口
    /// </summary>
    public interface ITopicProducer
    {
        /// <summary>
        /// 生产者ID
        /// </summary>
        string ProducerId { get; }
        /// <summary>
        /// MQ相关配置
        /// </summary>
        RabbitMqConfig Config { get; }
        /// <summary>
        /// 发布消息 不抛异常表示发送成功
        /// </summary>
        /// <param name="topic">指定消息的topic</param>
        /// <param name="key">消息的Key</param>
        /// <param name="content">消息内容</param>
        void Publish(string topic, string key, string content);
        /// <summary>
        /// 带确认机制的发送  效率比不带确认的发送低 不抛异常表示发送成功
        /// </summary>
        /// <param name="topic">指定消息的topic</param>
        /// <param name="key">消息的Key</param>
        /// <param name="content">消息内容</param>
        void PublishWithConfirm(string topic, string key, string content);
    }
}
