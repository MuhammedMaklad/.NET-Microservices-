namespace PlatformService.AsyncDataServices.MessageBroker
{
  public class RabbitMQSetting
  {
    public string? RabbitMQHost { get; set; }
    public string? RabbitMQPort { get; set; }
    public string? UserName { get; set; }
    public string? Password { get; set; }
  }

  //RabbitMQ Queue name
  public static class RabbitMQQueues
  {
    public const string CreatePlatformQueue = "createPlatformQueue";
    public const string AnotherQueue = "anotherQueue";
    public const string ThirdQueue = "thirdQueue";
  }

}