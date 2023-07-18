using commandservice.Models;
using commandservice.SyncDataServices.Grpc;

namespace commandservice.Data
{
    public static class PrepDb
    {
        public static void PrepPopulation(IApplicationBuilder applicationBuilder)
        {
            using(var servicescope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var grpcClient = servicescope.ServiceProvider.GetService<IPlatformDataClient>();
                var platforms = grpcClient.ReturnAllPlatforms();
                SeedData(servicescope.ServiceProvider.GetService<ICommandRepo>(), platforms ?? new List<Platform>());
            }
        }

        private static void SeedData(ICommandRepo repo, IEnumerable<Platform> platforms)
        {
            Console.WriteLine("Seeding New Platforms....>");

            foreach (var platform in platforms)
            {
                if(!repo.ExternalPlatformExists(platform.ExternalId))
                {
                    repo.CreatePlatform(platform);
                }
                repo.SaveChanges();
            }
        }
    }
}
