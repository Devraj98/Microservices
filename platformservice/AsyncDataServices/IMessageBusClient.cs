using PlatformService.Dtos;

namespace platformservice.AsyncDataServices
{
    public interface IMessageBusClient
    {
        void PublishNewPlatform(PlatformPublishedDto platformPublishedDto);
    }
}
