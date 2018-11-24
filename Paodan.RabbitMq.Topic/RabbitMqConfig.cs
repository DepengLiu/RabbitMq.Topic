namespace Paodan.RabbitMq.Topic
{
    public class RabbitMqConfig
    {
        /// <summary>
        /// 服务器IP地址
        /// </summary>
        public string HostName { get; set; }

        /// <summary>
        /// 服务器端口，默认是 5672
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// 登录用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 登录密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 虚拟主机名称
        /// </summary>
        public string VirtualHost { get; set; }

        /// <summary>
        /// Exchange名称
        /// </summary>
        public string ExchangeName { get; set; }

    }
}
