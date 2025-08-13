using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Dtos;
using PlatformService.Repository;
using PlatformService.Models;
using PlatformService.SyncDataServices.Http;

namespace PlatformService;

[ApiController]
[Route("api/[controller]")]
public class PlatformController : ControllerBase
{
  private readonly IPlatformRepository _repository;
  private readonly IMapper _mapper;
  private readonly ILogger<PlatformController> _logger;

  private readonly ICommandDataClient _commandDataClient;
  public PlatformController(
    IPlatformRepository repository,
    IMapper mapper,
    ILogger<PlatformController> logger,
    ICommandDataClient commandDataClient
  )
  {
    _mapper = mapper;
    _repository = repository;
    _logger = logger;
    _commandDataClient = commandDataClient;
  }

  [HttpGet]
  public ActionResult<IEnumerable<ReadPlatformDto>> GetPlatforms()
  {
    var platforms = _repository.GetAllPlatforms();
    return Ok(_mapper.Map<IEnumerable<ReadPlatformDto>>(platforms));
  }

  [HttpGet("{id:int}", Name = "GetPlatformById")]
  public ActionResult<ReadPlatformDto> GetPlatformById(int id)
  {
    _logger.LogInformation($"Getting platform by ID: {id}");

    var platform = _repository.GetPlatformById(id);
    if (platform is null)
      return NotFound("Invalid id");
    return Ok(_mapper.Map<ReadPlatformDto>(platform));
  }

  [HttpPost]
  public async Task<ActionResult<ReadPlatformDto>> CreatePlatform([FromBody] CreatePlatformDto platformDto)
  {
    if (!ModelState.IsValid)
      return BadRequest(ModelState);

    var platform = _mapper.Map<Platform>(platformDto);
    _repository.CreatePlatform(platform);
    var isSaved = _repository.SaveChanges();

    var platformReadDto = _mapper.Map<ReadPlatformDto>(platform);

    try
    {
      await _commandDataClient.SendPlatformToCommand(platformReadDto);
     
    }
    catch (Exception ex)
    {
      System.Console.WriteLine($"---> Couldn't Send Synchronuslly {ex.Message}");
    }
    
    if (isSaved)
      return CreatedAtAction(
            nameof(GetPlatformById),
            new { id = platformReadDto.Id },
            platformReadDto);
            
    return StatusCode(500, "Failed to create platform");
  }
}
