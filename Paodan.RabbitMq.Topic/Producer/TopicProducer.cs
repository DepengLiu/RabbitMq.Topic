using RabbitMQ.Client;
using System;
using System.Text;

namespace Paodan.RabbitMq.Topic.Producer
{
    /// <summary>
    /// 基于Topic的生产者接口的实现类
    /// </summary>
    public class TopicProducer : ITopicProducer, IDisposable
    {
        /// <summary>
        /// MQ连接对象
        /// </summary>
        public IConnection Connection { get; private set; }
        /// <summary>
        /// Channel
        /// </summary>
        public IModel Channel { get; private set; }
        /// <summary>
        /// 生产者ID
        /// </summary>
        public string ProducerId { get; private set; }
        /// <summary>
        /// MQ相关配置
        /// </summary>
        public RabbitMqConfig Config { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="producerId">生产者ID</param>
        /// <param name="config">MQ相关配置</param>
        internal TopicProducer(string producerId, RabbitMqConfig config)
        {
            ProducerId = producerId;
            Config = config;
        }
        /// <summary>
        /// 发布消息 不抛异常表示发送成功
        /// </summary>
        /// <param name="topic">指定消息的topic</param>
        /// <param name="key">消息的Key</param>
        /// <param name="content">消息内容</param>
        public void Publish(string topic, string key, string content)
        {
            CheckConnection();
            var properties = Channel.CreateBasicProperties();
            properties.DeliveryMode = 2;

            var message = new Message() { Key = key, Content = content };
            var body = Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(message));

            Channel.BasicPublish(exchange: Config.ExchangeName, routingKey: topic, basicProperties: properties, body: body);
            Console.WriteLine($"{ProducerId}发送消息成功,Topic:{topic},Key:{key}");
        }

        /// <summary>
        /// 带确认机制的发送  效率比不带确认的发送低 不抛异常表示发送成功
        /// </summary>
        /// <param name="topic">指定消息的topic</param>
        /// <param name="key">消息的Key</param>
        /// <param name="content">消息内容</param>
        public void PublishWithConfirm(string topic, string key, string content)
        {
            CheckConnection();
            var properties = Channel.CreateBasicProperties();
            properties.DeliveryMode = 2;
            var message = new Message() { Key = key, Content = content };
            var body = Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(message));
            try
            {
                Channel.TxSelect();
                Channel.BasicPublish(exchange: Config.ExchangeName, routingKey: topic, basicProperties: properties, body: body);
                Channel.TxCommit();
                Console.WriteLine($"{ProducerId}发送消息成功,Topic:{topic},Key:{key}");
            }
            catch
            {
                Channel.TxRollback();
                Console.WriteLine($"{ProducerId}发送消息失败,Topic:{topic},Key:{key}");
                throw;
            }
        }

        public void Dispose()
        {
            if (Channel != null)
            {
                try
                {
                    Channel.Close();
                    Channel.Dispose();

                }
                catch { }

            }
            if (Connection != null)
            {
                try
                {
                    Connection.Close();
                    Connection.Dispose();
                }
                catch { }
            }
        }

        private void CheckConnection()
        {
            //延迟到发布时创建连接，避免服务不可用时 无法创建对象
            if (Connection == null)
            {
                Connection = MqConnFactory.CreateConnection(Config);
                Connection.RecoverySucceeded += Connection_RecoverySucceeded;
                Connection.ConnectionShutdown += Connection_ConnectionShutdown;
                Channel = Connection.CreateModel();
                Channel.ExchangeDeclare(exchange: Config.ExchangeName, type: ExchangeType.Topic, durable: true, autoDelete: false, arguments: null);
            }
            if (!Connection.IsOpen)
            {
                throw new Exception($"连接已被关闭,原因:{Connection.CloseReason.ReplyText}");
            }
        }

        private void Connection_RecoverySucceeded(object sender, EventArgs e)
        {
            Console.WriteLine("断线重连成功");
        }

        private void Connection_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            Console.WriteLine($"连接关闭,{e.ReplyText},{e.Initiator}");
        }
    }
}
