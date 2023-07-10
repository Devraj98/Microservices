using commandservice.Models;

namespace commandservice.SyncDataServices.Grpc
{
    public interface IPlatformDataClient
    {
        IEnumerable<Platform> ReturnAllPlatforms();
    }
}
