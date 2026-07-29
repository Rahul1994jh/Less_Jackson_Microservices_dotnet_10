using PlatformService.Models;

namespace PlatformService.SyncDataService.Http;

public interface ICommandDataClient
{
    Task SendPlatformToCommand(PlatformReadDto platform);
}

