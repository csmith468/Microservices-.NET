using PlatformService.DTOs;

namespace PlatformService.SyncDataServices.HTTP
{
    public interface ICommandDataClient
    {
        Task SendPlatformToCommand(PlatformReadDto plat);
    }
}