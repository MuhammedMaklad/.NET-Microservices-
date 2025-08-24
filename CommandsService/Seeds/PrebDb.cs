using System;
using System.Collections.Generic;
using CommandsService.Models;
using CommandsService.Repository.IRepository;
using CommandsService.SyncDataServices.Grpc;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace CommandsService.Data
{
    public static class PrepDb
    {
        public static void PrepPopulation(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var grpcClient = serviceScope.ServiceProvider.GetService<IPlatformDataClient>();

                var platforms = grpcClient?.ReturnAllPlatforms()!;

                        // Ensure platforms is never null
                  platforms ??= new List<Platform>();

                SeedData(serviceScope.ServiceProvider.GetService<ICommandRepository>()!, platforms);
            }
        }
        
        private static void SeedData(ICommandRepository repo, IEnumerable<Platform> platforms)
        {
            Console.WriteLine("Seeding new platforms...");

            foreach (var plat in platforms)
            {
                if(!repo.ExternalPlatformExist(plat.ExternalId))
                {
                    repo.CreatePlatform(plat);
                }
                repo.saveChanges();
            }
        }
    }
}