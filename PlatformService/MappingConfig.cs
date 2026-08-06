using Mapster;
using PlatformService.Models;

namespace PlatformService;

public static class MappingConfig
{
    public static void RegisterMappings()
    {
        TypeAdapterConfig<Platform, GrpcPlatformModel>
            .NewConfig()
            .Map(dest => dest.PlatformId, src => src.Id);
    }
}
