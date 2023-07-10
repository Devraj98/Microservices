using AutoMapper;
using commandservice.Data;
using commandservice.Dtos;
using commandservice.Models;
using System.Text.Json;
using System.Windows.Input;

namespace commandservice.EventProcessing
{
    public class EventProcessor : IEventProcessor
    {
        private IServiceScopeFactory _scopeFactory;
        private IMapper _mapper;

        public EventProcessor(IServiceScopeFactory scopeFactory, IMapper mapper)
        {
            _scopeFactory = scopeFactory;
            _mapper = mapper;
        }

        public void ProcessEvent(string message)
        {
            if(DetermineEvent(message) == EventType.PlatformPublished)
            {
                AddPlatform(message);
            }
        }

        private EventType DetermineEvent(string notificationMessage)
        {
            Console.WriteLine("--> Determining Event");
            var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);

            switch (eventType?.Event)
            {
                case "Platform_Published":
                    Console.WriteLine("Platform Published Event Detected");
                    return EventType.PlatformPublished;

                default:
                    Console.WriteLine("--> Couldn't determine the event Type");
                    return EventType.Undetermined;
            }
        }

        private void AddPlatform(string platformPublishedMessage)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<ICommandRepo>();

                var platformPublishedDto = JsonSerializer.Deserialize<PlatformPublishedDto>(platformPublishedMessage);
                try
                {
                    var plat = _mapper.Map<Platform>(platformPublishedDto);
                    if (!repo.ExternalPlatformExists(plat.ExternalId))
                    {
                        repo.CreatePlatform(plat);
                        repo.SaveChanges();
                    }
                    else
                    {
                        Console.WriteLine("--> Platform already Exists");
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"Couldn't add Platform to DB , {ex.Message}");
                }
            
            }
        }
    }



    enum EventType
    {
        PlatformPublished,
        Undetermined
    }
}
