using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace Paodan.RabbitMq.Topic.Consumer
{
    /// <summary>
    /// 基于Topic的消费者接口的实现类
    /// </summary>
    public class TopicConsumer : ITopicConsumer, IDisposable
    {
        /// <summary>
        /// MQ相关配置
        /// </summary>
        public RabbitMqConfig Config { get; private set; }
        /// <summary>
        /// 消费者ID
        /// </summary>
        public string ConsumerId { get; private set; }
        /// <summary>
        /// MQ连接对象
        /// </summary>
        public IConnection Connection { get; private set; }
        /// <summary>
        /// 是否持久化  如果要支持离线重连后继续消费离线期间的消息，必须要支持持久化
        /// </summary>
        public bool Durable { get; private set; }
        /// <summary>
        /// 有消息到达时触发
        /// </summary>
        public event MessageReceiveEventHandler MessageReceive;

        /// <summary>
        /// Channel
        /// </summary>
        public IModel Channel { get; private set; }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="consumerId">消费者ID 也是队列名称</param>
        /// <param name="config">MQ相关配置</param>
        /// <param name="durable">是否持久化  如果要支持离线重连后继续消费离线期间的消息，必须要支持持久化</param>
        internal TopicConsumer(string consumerId, RabbitMqConfig config, bool durable)
        {
            ConsumerId = consumerId;
            Config = config;
            Durable = durable;
        }

        /// <summary>
        /// 订阅Topic
        /// </summary>
        /// <param name="topics">要订阅的topic</param>
        public void Subscribe(params string[] topics)
        {
            if (topics == null || topics.Length <= 0) throw new ArgumentException("至少需要指定一个Topic");
            CheckConnection();
            foreach (var topic in topics)
            {
                Channel.QueueBind(queue: ConsumerId, exchange: Config.ExchangeName, routingKey: topic);
            }
            var consumer = new EventingBasicConsumer(Channel);
            //注册接收事件，一旦创建连接就去拉取消息
            consumer.Received += (sender, e) =>
            {
                try
                {
                    string body = Encoding.UTF8.GetString(e.Body);
                    Message message = Newtonsoft.Json.JsonConvert.DeserializeObject<Message>(body);
                    MessageReceiveEventArgs args = new MessageReceiveEventArgs(e.RoutingKey, message.Key, message.Content);
                    if (MessageReceive != null)
                    {
                        MessageReceive(this, args);
                    }
                    if (args.Result == ConsumeResult.Success)
                    {
                        //回复确认处理成功
                        Channel.BasicAck(e.DeliveryTag, false);
                    }
                    else if (args.Result == ConsumeResult.Retry)
                    {
                        //发生错误了，但是还可以重新提交给队列重新分配
                        Channel.BasicNack(e.DeliveryTag, false, true);
                    }
                    else
                    {
                        Channel.BasicNack(e.DeliveryTag, false, false);
                    }
                }
                catch
                {
                    Channel.BasicNack(e.DeliveryTag, false, true);
                }
            };
            Console.WriteLine($"{ConsumerId} 已就绪");
            Channel.BasicConsume(ConsumerId, false, consumer);
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
            //延迟到订阅时创建连接，避免服务不可用时 无法创建对象
            if (Connection == null)
            {
                Connection = MqConnFactory.CreateConnection(Config);
                Connection.RecoverySucceeded += Connection_RecoverySucceeded;
                Connection.ConnectionShutdown += Connection_ConnectionShutdown;
                Channel = Connection.CreateModel();
                Channel.ExchangeDeclare(exchange: Config.ExchangeName, type: ExchangeType.Topic, durable: true, autoDelete: false, arguments: null);
                Channel.QueueDeclare(ConsumerId, true, false, false, null);
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
            Console.WriteLine($"连接关闭,{e.Cause},{e.Initiator}");
        }
    }
}
