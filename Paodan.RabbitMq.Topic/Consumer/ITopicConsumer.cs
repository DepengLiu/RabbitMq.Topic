using System;

namespace Paodan.RabbitMq.Topic.Consumer
{
    /// <summary>
    /// 基于Topic的消费者接口
    /// </summary>
    public interface ITopicConsumer
    {
        /// <summary>
        /// MQ相关配置
        /// </summary>
        RabbitMqConfig Config { get; }
        /// <summary>
        /// 消费者ID
        /// </summary>
        string ConsumerId { get; }
        /// <summary>
        /// 是否持久化  如果要支持离线重连后继续消费离线期间的消息，必须要支持持久化
        /// </summary>
        bool Durable { get; }
        /// <summary>
        /// 订阅Topic 可以使用通配符 * #。  如：log.*,log.#
        /// </summary>
        /// <param name="topics"></param>
        void Subscribe(params string[] topics);
        /// <summary>
        /// 有消息到达时触发
        /// </summary>
        event MessageReceiveEventHandler MessageReceive;
    }

    /// <summary>
    /// 消息到达时的事件参数
    /// </summary>
    public class MessageReceiveEventArgs : EventArgs
    {
        /// <summary>
        /// 消息的topic
        /// </summary>
        public string Topic { get; private set; }
        /// <summary>
        /// 消息的key
        /// </summary>
        public string Key { get; private set; }
        /// <summary>
        /// 消息内容
        /// </summary>
        public string Content { get; private set; }
        /// <summary>
        /// 消费结果   
        /// </summary>
        public ConsumeResult Result { get; set; } = ConsumeResult.Retry;

        public MessageReceiveEventArgs(string topic, string key, string content)
        {
            Topic = topic;
            Key = key;
            Content = content;
        }
    }
    /// <summary>
    /// 消息到达时的事件委托
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void MessageReceiveEventHandler(ITopicConsumer sender, MessageReceiveEventArgs e);
}
