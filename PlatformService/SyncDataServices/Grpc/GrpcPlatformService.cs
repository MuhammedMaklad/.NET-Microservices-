using AutoMapper;
using Grpc.Core;
using PlatformService.Protos;
using PlatformService.Repository;

namespace PlatformService.SyncDataServices.Grpc;

public class GrpcPlatformService : GrpcPlatform.GrpcPlatformBase
{
  private readonly ILogger<GrpcPlatformService> _logger;
  private readonly IMapper _mapper;
  private IPlatformRepository _platformRepository;
  public GrpcPlatformService(
    ILogger<GrpcPlatformService> logger,
    IMapper mapper,
    IPlatformRepository platformRepository
    )
  {
    _logger = logger;
    _mapper = mapper;
    _platformRepository = platformRepository;
  }

  public override Task<GetAllPlatformResponse> GetAllPlatform(GetAllPlatformRequest request, ServerCallContext context)
  {
    var response = new GetAllPlatformResponse();
    var platforms = _platformRepository.GetAllPlatforms();

    foreach (var platform in platforms)
    {
      response.Platforms.Add(_mapper.Map<GrpcPlatformModel>(platform));
    }
    return Task.FromResult(response);
  }
}