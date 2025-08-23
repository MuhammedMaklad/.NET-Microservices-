using System.Text;
using CommandsService.EventProcessing;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CommandsService.AsyncDataServices.MessageBroker;

// Background service that subscribes to RabbitMQ message bus
public class MessageBusSubscriber : BackgroundService
{
  // Configuration and dependency injection fields
  private readonly RabbitMQSettings _rabbitMqSettings; // RabbitMQ configuration
  private readonly ILogger<MessageBusSubscriber> _logger; // Logger for service events
  private IConnection? _connection; // RabbitMQ connection
  private IModel? _channel; // RabbitMQ channel
  private string? _queueName; // Queue name for subscribing
  private readonly IEventProcessor _eventProcessor; // Processor for handling received events

  // Constructor with dependency injection
  public MessageBusSubscriber(
    IOptions<RabbitMQSettings> rabbitMqSettings, // Injected RabbitMQ settings
    ILogger<MessageBusSubscriber> logger,
    IEventProcessor eventProcessor
  )
  {
    _rabbitMqSettings = rabbitMqSettings?.Value ?? throw new ArgumentException(nameof(rabbitMqSettings));
    _logger = logger;
    _eventProcessor = eventProcessor;

    InitializeRabbitMQ(); // Initialize RabbitMQ connection on construction
  }

  // Initializes RabbitMQ connection, channel, exchange, and queue
  private void InitializeRabbitMQ()
  {
    var factory = new ConnectionFactory()
    {
      HostName = _rabbitMqSettings.RabbitMQHost,
      Port = Convert.ToInt32(_rabbitMqSettings.RabbitMQPort),
      Password = _rabbitMqSettings.RabbitMQPassword,
      UserName = _rabbitMqSettings.RabbitMQUsername,
    };
    
    _connection = factory.CreateConnection(); // Create connection
    _channel = _connection.CreateModel(); // Create channel

    // Declare a fanout exchange named "trigger"
    _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);

    // Declare a queue and get the generated queue name
    _queueName = _channel.QueueDeclare().QueueName;

    // Bind the queue to the exchange with no routing key (fanout pattern)
    _channel.QueueBind(queue: _queueName,
    exchange: "trigger",
    routingKey: "");

    _logger.LogInformation("Listenting on the Message Bus");
    
    // Register connection shutdown event handler
    _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown!;
  }

  // Main execution method for the background service
  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    stoppingToken.ThrowIfCancellationRequested(); // Check if cancellation requested
    StartConsuming(_queueName!, stoppingToken); // Start consuming messages
    await Task.CompletedTask; // Return completed task (service runs indefinitely)
  }

  // Starts consuming messages from the specified queue
  private void StartConsuming(string queueName, CancellationToken cancellationToken)
  {
    var consumer = new EventingBasicConsumer(_channel); // Create event-based consumer

    // Event handler for when a message is received
    consumer.Received += (ModuleHandle, ea) =>
    {
      _logger.LogInformation("Event Received!");

      var body = ea.Body; // Get message body
      var notificationMessage = Encoding.UTF8.GetString(body.ToArray()); // Convert to string

      _eventProcessor.ProcessEvent(notificationMessage); // Process the received event
    };

    // Start consuming messages with automatic acknowledgement
    _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);
  }

  // Event handler for RabbitMQ connection shutdown
  private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
  {
    _logger.LogInformation("--> Connection Shutdown"); // Log connection shutdown
  }

  // Cleanup method to properly dispose of RabbitMQ resources
  public override void Dispose()
  {
    if(_channel?.IsOpen == true) // Check if channel is open
    {
      _channel.Close(); // Close channel
      _connection?.Close(); // Close connection
    }
    base.Dispose(); // Call base dispose
  }
}