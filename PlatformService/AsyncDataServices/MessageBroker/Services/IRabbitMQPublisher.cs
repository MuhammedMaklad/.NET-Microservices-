namespace PlatformService.AsyncDataServices.MessageBroker.Services;

public interface IRabbitMQPublisher<T>
{
  Task PublishMessageAsync(T Message, string queueName);
}