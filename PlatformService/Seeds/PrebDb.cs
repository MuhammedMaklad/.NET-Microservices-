
using Microsoft.EntityFrameworkCore;
using PlatformService.Data;
using PlatformService.Models;

namespace PlatformService.Seeds
{
  public static class PrepDb
  {
      public static void PrepPopulation(IApplicationBuilder app, bool isProd)
      {
          using var serviceScope = app.ApplicationServices.CreateScope();
          var dbContext = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();
          
          ApplyMigrations(dbContext, isProd);
          SeedInitialData(dbContext);
      }

      private static void ApplyMigrations(AppDbContext dbContext, bool isProd)
      {
          if (!isProd) 
              return;

          Console.WriteLine("--> Attempting to apply migrations...");
          
          try
          {
              dbContext.Database.Migrate();
          }
          catch (Exception ex)
          {
              Console.WriteLine($"--> Could not run migrations: {ex.Message}");
              throw; // Re-throw to ensure the app fails fast if migrations are critical
          }
      }

      private static void SeedInitialData(AppDbContext dbContext)
      {
          if (dbContext.Platforms.Any())
          {
              Console.WriteLine("--> Data already exists. Skipping seeding.");
              return;
          }

          Console.WriteLine("--> Seeding initial data...");

          var platforms = new[]
          {
              new Platform { Name = "Dot Net", Publisher = "Microsoft", Cost = "Free" },
              new Platform { Name = "SQL Server Express", Publisher = "Microsoft", Cost = "Free" },
              new Platform { Name = "Kubernetes", Publisher = "Cloud Native Computing Foundation", Cost = "Free" }
          };

          dbContext.Platforms.AddRange(platforms);
          dbContext.SaveChanges();
      }
  }
}