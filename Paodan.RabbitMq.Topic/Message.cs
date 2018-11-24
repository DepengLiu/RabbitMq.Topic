namespace Paodan.RabbitMq.Topic
{
    /// <summary>
    /// 内部使用消息类  包装Key和Content
    /// </summary>
    internal class Message
    {
        public string Key { get; set; }
        public string Content { get; set; }
    }
}
