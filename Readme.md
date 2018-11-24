# 基于Topic方式的RabbitMQ简单封装

## 消息生产者
```CSHARP 
RabbitMqConfig config = new RabbitMqConfig();
config.ExchangeName = "test";
config.HostName = "localhost";
config.Password = "guest";
config.UserName = "guest";
config.VirtualHost = "/";
var factory = new ProducerFactory(config);
var producer = factory.CreateProducer("Producer1");
producer.Publish("topic", "key", "message");
```  
##  消息消费者
```CSHARP
RabbitMqConfig config = new RabbitMqConfig();
config.ExchangeName = "test";
config.HostName = "localhost";
config.Password = "guest";
config.UserName = "guest";
config.VirtualHost = "/";

ConsumerFactory factory = new ConsumerFactory(config);
var consumer = factory.CreateConsumer("ConsumerId1", true);
consumer.MessageReceive += (sender, e) =>
 {
    Console.WriteLine($"{sender.ConsumerId}收到消息,Topic:{e.Topic}:Key:{e.Key},Content:{e.Content}");
    e.Result = ConsumeResult.Success;
 };
consumer.Subscribe("topic");
```