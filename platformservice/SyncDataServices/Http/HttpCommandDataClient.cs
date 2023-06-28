using PlatformService.Dtos;
using System.Text;
using System.Text.Json;

namespace platformservice.SyncDataServices.Http
{
    public class HttpCommandDataClient : ICommandDataClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public HttpCommandDataClient(HttpClient httpClient, IConfiguration config) 
        {
            _httpClient = httpClient;
            _config = config;
        }

        public async Task SendPlatformToCommand(PlatformReadDto platformReadDto)
        {
            var httpContent = new StringContent(
                JsonSerializer.Serialize(platformReadDto),
                Encoding.UTF8,
                "application/json");
            Console.WriteLine($"{_config["CommandService"]}/api/c/Platforms");
            var response = await _httpClient.PostAsync($"{_config["CommandService"]}/api/c/Platforms", httpContent);
            if(response.IsSuccessStatusCode)
            {
                Console.WriteLine("--> Sync POST to CommandService was OK!");
            }
            else 
            {
                Console.WriteLine("--> Sync POST to CommandService failed!");
            }
        }
    }
}
