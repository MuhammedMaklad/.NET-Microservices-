using AutoMapper;
using CommandsService.Models;
using CommandsService.Protos;
using Grpc.Net.Client;

namespace CommandsService.SyncDataServices.Grpc;

public class PlatformDataClient : IPlatformDataClient
{
  private readonly ILogger<PlatformDataClient> _logger;
  private readonly IConfiguration _configuration;
  private readonly IMapper _mapper;
  public PlatformDataClient(ILogger<PlatformDataClient> logger, IConfiguration configuration, IMapper mapper)
  {
    _logger = logger;
    _configuration = configuration;
    _mapper = mapper;
  }
  public IEnumerable<Platform> ReturnAllPlatforms()
  {
    _logger.LogInformation($"Calling Grpc Service {_configuration["GrpcPlatformURI"]}");

    var channel = GrpcChannel.ForAddress(_configuration["GrpcPlatformURI"]!);

    var client = new GrpcPlatform.GrpcPlatformClient(channel);

    var request = new GetAllPlatformRequest();

    try
    {
      var response = client.GetAllPlatform(request);
      return _mapper.Map<IEnumerable<Platform>>(response.Platforms);
    }
    catch (Exception ex)
    {
      _logger.LogError($"Error while Request platforms from grpc {ex.Message}");
      return null!;
    }
  }
}