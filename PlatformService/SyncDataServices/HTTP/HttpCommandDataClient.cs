using System.Text;
using System.Text.Json;
using PlatformService.DTOs;

namespace PlatformService.SyncDataServices.HTTP
{
    public class HttpCommandDataClient: ICommandDataClient 
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public HttpCommandDataClient(HttpClient httpClient, IConfiguration config) 
        {
            _httpClient = httpClient;
            _config = config;
        }

        public async Task SendPlatformToCommand(PlatformReadDto plat)
        {
            Console.WriteLine($"--> CommandService Endpoint: {_config["CommandService"]}");
            var httpContent = new StringContent(
                JsonSerializer.Serialize(plat),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync(
                $"{_config["CommandService"]}", httpContent);

            if (response.IsSuccessStatusCode) 
            {
                Console.WriteLine("--> Sync POST to CommandService was OK!");
            }
            else 
            {
                Console.WriteLine("--> Sync POST to CommandService was NOT OK!");
            }
        }
    }
}