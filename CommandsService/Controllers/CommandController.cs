using AutoMapper;
using CommandsService.Models;
using CommandsService.Repository.IRepository;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers
{
  [Route("/api/c/platforms/{platformId:int}/[controller]")]
  [ApiController]
  public class CommandController : ControllerBase
  {
    private readonly ICommandRepository _commandRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<PlatformController> _logger;
    public CommandController(
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

    [HttpGet(Name = "GetCommandsForPlatform")]
    public ActionResult<IEnumerable<CommandReadDto>> GetAllCommand(int platformId)
    {
      if (!(_commandRepository.PlatformExist(platformId)))
        return NotFound("Invalid Platform Id");
      var commands = _commandRepository.GetCommandsForPlatform(platformId);

      return Ok(_mapper.Map<IEnumerable<CommandReadDto>>(commands));
    }

    [HttpGet("{commandId:int}", Name = "GetCommandForPlatform")]
    public ActionResult<CommandReadDto> GetCommandForPlatform(int platformId, int commandId)
    {
      var command = _commandRepository.GetCommandForPlatform(platformId, commandId);
      if (command is null)
        return NotFound();
      return Ok(_mapper.Map<CommandReadDto>(command));
    }

    [HttpPost("{platformId:int}", Name = "CreateCommandForPlatform")]
    public ActionResult<CommandReadDto> CreateCommand(int platformId, CommandCreateDto commandCreateDto)
    {
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      if (!_commandRepository.PlatformExist(platformId))
        return NotFound("Invalid Platform Id");

      var command = _mapper.Map<Command>(commandCreateDto);

      _commandRepository.CreateCommand(platformId, command);

      var done = _commandRepository.saveChanges();
      if(done is false)
        return Problem("A database error occurred while saving the command.", statusCode: 500);

      var commandReadDto = _mapper.Map<CommandReadDto>(command);
      return CreatedAtRoute(nameof(GetCommandForPlatform),
       new { platformId = platformId, commandId=commandReadDto.Id}, commandReadDto);
    }
  }
}

