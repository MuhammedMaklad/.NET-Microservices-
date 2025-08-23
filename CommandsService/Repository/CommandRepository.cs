using CommandsService.Data;
using CommandsService.Models;
using CommandsService.Repository.IRepository;

namespace CommandsService.Repository;

public class CommandRepository : ICommandRepository
{
  private readonly ApplicationDbContext _context;
  public CommandRepository(ApplicationDbContext context)
  {
    _context = context;
  }
  public void CreateCommand(int platformId, Command command)
  {
    command.PlatformId = platformId;
    _context.Commands.Add(command);
  }

  public void CreatePlatform(Platform platform)
  {
    _context.Platforms.Add(platform);
  }

  public IEnumerable<Platform> GetAllPlatforms()
  {
    return _context.Platforms.ToList();
  }

  public Command? GetCommandForPlatform(int platformId, int commandId)
  {
    return _context.Commands.FirstOrDefault(x => x.Id == commandId && x.PlatformId == platformId);
  }

  public IEnumerable<Command> GetCommandsForPlatform(int platformId)
  {
    return _context.Commands.Where(x => x.PlatformId == platformId).ToList();
  }

  public bool PlatformExist(int platformId)
  {
    return _context.Platforms.FirstOrDefault(x => x.Id == platformId) is not null;
  }
  public bool ExternalPlatformExist(int externalPlatformId)
  {
    return _context.Platforms.FirstOrDefault(x => x.ExternalId == externalPlatformId) is not null;
  }

  public bool saveChanges()
  {
    return _context.SaveChanges() > 0;
  }
}
