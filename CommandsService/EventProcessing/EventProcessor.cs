using System.Text.Json;
using AutoMapper;
using CommandsService.Dtos;
using CommandsService.EventProcessing;
using CommandsService.Models;
using CommandsService.Repository;
using CommandsService.Repository.IRepository;

namespace CommandsService;

// EventProcessor class responsible for processing incoming events/messages
public class EventProcessor : IEventProcessor
{
  // Dependency injection fields
  private readonly IServiceScopeFactory _scopeFactory; // Creates scopes for service resolution
  private readonly IMapper _mapper; // AutoMapper for object mapping
  private readonly ILogger<EventProcessor> _logger; // Logger for logging events

  // Constructor with dependency injection
  public EventProcessor(
    IServiceScopeFactory scopeFactory,
    IMapper mapper,
    ILogger<EventProcessor> logger
    )
  {
    _scopeFactory = scopeFactory;
    _mapper = mapper;
    _logger = logger;
  }

  // Main method to process incoming events
  public void ProcessEvent(string message)
  {
    var eventType = DetermineEvent(message); // Determine what type of event this is
    switch (eventType)
    {
      case EventType.PlatformPublished:
        // TODO: 
        AddPlatform(message); // Process platform published event
        break;
      default:
        break; // Ignore undetermined events
    }
  }

  // Determines the type of event based on message content
  private EventType DetermineEvent(string notificationMessage)
  {
    _logger.LogInformation("Determining Event");

    // Deserialize the generic event DTO to check the event type
    var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);
    
    switch (eventType?.Event)
    {
      case "Platform-Publish":
        _logger.LogInformation($"Platform Published Event Detect {EventType.PlatformPublished}");
        return EventType.PlatformPublished;
      default:
        _logger.LogInformation($"Platform Published Event UnDetect");
        return EventType.Undetermined;
    }
  }

  // Processes platform published events by adding platforms to the database
  private void AddPlatform(string platformPublishedMessage)
  {
    // Create a scope to resolve scoped services (like repositories)
    using (var scope = _scopeFactory.CreateScope())
    {
      // Get the repository from the service provider
      var repo = scope.ServiceProvider.GetRequiredService<ICommandRepository>();

      // Deserialize the platform message
      var platformPublishDto = JsonSerializer.Deserialize<PlatformPublishDto>(platformPublishedMessage);

      try
      {
        // Map DTO to domain model using AutoMapper
        var platform = _mapper.Map<Platform>(platformPublishDto);
        
        // Check if platform already exists to avoid duplicates
        if (!repo.ExternalPlatformExist(platform.ExternalId))
        {
          repo.CreatePlatform(platform); // Create new platform
          repo.saveChanges(); // Save changes to database
        }
        else
        {
          _logger.LogInformation("Platform Already Exist"); // Log if platform exists
        }
      }
      catch (Exception ex)
      {
        _logger.LogError($"Couldn't Add platform To DB {ex.Message}"); // Log any errors
      }
    }
  }
}

// Enum defining possible event types
enum EventType
{
  PlatformPublished, // Event for when a platform is published
  Undetermined      // Default for unknown events
}