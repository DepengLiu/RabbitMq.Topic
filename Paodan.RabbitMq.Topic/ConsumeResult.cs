namespace Paodan.RabbitMq.Topic
{
    public enum ConsumeResult
    {
        /// <summary>
        /// 处理成功
        /// </summary>
        Success,

        /// <summary>
        /// 重发
        /// </summary>
        Retry,

        /// <summary>
        /// 无需重试的错误
        /// </summary>
        Reject,
    }
}
