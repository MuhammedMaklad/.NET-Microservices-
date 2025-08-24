using AutoMapper;
using CommandsService.Dtos;
using CommandsService.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers
{
  [Route("/api/c/[controller]")]
  [ApiController]
  public class PlatformController : ControllerBase
  {
    private readonly ICommandRepository _commandRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<PlatformController> _logger;
    public PlatformController(
     ICommandRepository commandRepository,
     IMapper mapper,
     ILogger<PlatformController> logger
     )
    {
      // Synchronous & Asynchronous Messaging 
      _commandRepository = commandRepository;
      _mapper = mapper;
      _logger = logger;
    }
    [HttpPost]
    public ActionResult TestInboundConnection()
    {
      _logger.LogInformation($"---> Inbound Post # Command Service");
      return Ok("Inbound test of from Platforms Controller");
    }

    [HttpGet("Platforms")]
    public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
    {
      var platformItems = _commandRepository.GetAllPlatforms();
      return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platformItems));
    }
  }
}
