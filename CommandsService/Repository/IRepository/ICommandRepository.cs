using CommandsService.Models;

namespace CommandsService.Repository.IRepository;

public interface ICommandRepository
{
  bool saveChanges();

  IEnumerable<Platform> GetAllPlatforms();
  void CreatePlatform(Platform platform);

  bool PlatformExist(int platformId);

  IEnumerable<Command> GetCommandsForPlatform(int platformId);
  Command? GetCommandForPlatform(int platformId, int commandId);
  void CreateCommand(int platformId, Command command);
}
