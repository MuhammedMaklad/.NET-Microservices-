
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace PlatformService.AsyncDataServices.MessageBroker.Services;

public class RabbitMQPublisher<T> : IRabbitMQPublisher<T>, IDisposable
{
  private readonly RabbitMQSetting _rabbitMqSetting;
  private readonly ILogger<RabbitMQPublisher<T>> _logger;
  private readonly IConnection _connection;
  private readonly IModel _channel;
  private bool _disposed = false;

  public RabbitMQPublisher(IOptions<RabbitMQSetting> rabbitMqSetting, ILogger<RabbitMQPublisher<T>> logger)
  {
    _rabbitMqSetting = rabbitMqSetting?.Value ?? throw new ArgumentNullException(nameof(rabbitMqSetting));

    _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    try
    {
      var factory = new ConnectionFactory()
      {
        HostName = _rabbitMqSetting.RabbitMQHost,
        Port = Convert.ToInt32(_rabbitMqSetting.RabbitMQPort),
        // Consider adding these for better reliability
        AutomaticRecoveryEnabled = true,
        NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
      };

      _connection = factory.CreateConnection();
      _connection.ConnectionShutdown += OnConnectionShutdown;

      _channel = _connection.CreateModel();

      // Declare the exchange once during initialization
      _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout, durable: true);

      _logger.LogInformation("RabbitMQ connection established successfully");
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to establish RabbitMQ connection");
      throw;
    }
  }
  public async Task PublishMessageAsync(T message, string queueName)
  {
    if (_disposed)
      throw new ObjectDisposedException(nameof(RabbitMQPublisher<T>));

    if (message == null)
      throw new ArgumentNullException(nameof(message));

    if (string.IsNullOrWhiteSpace(queueName))
      throw new ArgumentException("Queue name cannot be null or empty", nameof(queueName));

    try
    {
      // Declare queue (idempotent operation)
      _channel.QueueDeclare(
        queue: queueName,
        durable: true,  // Changed to true for persistence
        exclusive: false,
        autoDelete: false,
        arguments: null);

      // Bind queue to exchange
      _channel.QueueBind(queue: queueName, exchange: "trigger", routingKey: "");

      var messageJson = JsonSerializer.Serialize(message);
      var body = Encoding.UTF8.GetBytes(messageJson);

      // Create basic properties for message persistence
      var properties = _channel.CreateBasicProperties();
      properties.Persistent = true;
      properties.MessageId = Guid.NewGuid().ToString();
      properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());

      // Publish synchronously (RabbitMQ client is not truly async)
      _channel.BasicPublish(
        exchange: "trigger",
        routingKey: "",
        basicProperties: properties,
        body: body);

      _logger.LogDebug("Message published to queue {QueueName}: {MessageId}", queueName, properties.MessageId);

      // Return completed task since BasicPublish is synchronous
      await Task.CompletedTask;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to publish message to queue {QueueName}", queueName);
      throw;
    }
  }

  private void OnConnectionShutdown(object? sender, ShutdownEventArgs e)
  {
    if (!_disposed)
    {
      _logger.LogWarning("RabbitMQ connection shutdown: {ReplyText} (Code: {ReplyCode})",
          e.ReplyText, e.ReplyCode);
    }
  }

  public void Dispose()
  {
    Dispose(true);
    GC.SuppressFinalize(this);
  }
  protected virtual void Dispose(bool disposing)
  {
    if (!_disposed && disposing)
    {
      try
      {
        _connection.ConnectionShutdown -= OnConnectionShutdown;
        
        if (_channel?.IsOpen == true)
        {
            _channel.Close();
        }
        
        if (_connection?.IsOpen == true)
        {
            _connection.Close();
        }
        
        _channel?.Dispose();
        _connection?.Dispose();
        
        _logger.LogInformation("RabbitMQ publisher disposed");
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error occurred while disposing RabbitMQ publisher");
      }
      finally
      {
        _disposed = true;
      }
    }
  }
}