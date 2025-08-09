using PlatformService.Data;
using PlatformService.Models;

namespace PlatformService.Repository;

public class PlatformRepository : IPlatformRepository
{
  private readonly AppDbContext _context;
  public PlatformRepository(AppDbContext context)
  {
    _context = context;
  }
  public void CreatePlatform(Platform plat)
  {
    if (plat is null)
      throw new ArgumentNullException(nameof(plat));
    var entity = _context.Add(plat);
  }

  public IEnumerable<Platform> GetAllPlatforms()
  {
    return _context.Platforms.ToList();
  }

  public Platform? GetPlatformById(int id)
  {
    return _context.Platforms.FirstOrDefault(x => x.Id == id);
  }

  public bool SaveChanges()
  { 
    return (_context.SaveChanges() > 0);
  }
}
