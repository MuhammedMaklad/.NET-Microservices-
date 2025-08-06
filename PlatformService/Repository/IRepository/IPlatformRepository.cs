using PlatformService.Models;

namespace PlatformService.Repository;

public interface IPlatformRepository
{
  bool SaveChanges();
  IEnumerable<Platform?> GetAllPlatforms();
  Platform? GetPlatformById(int id);
  void CreatePlatform(Platform plat);
}
