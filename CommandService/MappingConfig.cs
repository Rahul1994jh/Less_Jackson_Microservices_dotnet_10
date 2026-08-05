using CommandService.Models;
using Mapster;

namespace CommandService
{
    public static class MappingConfig
    {
        public static void RegisterMappings()
        {
            TypeAdapterConfig<PlatformPublishedDto, Platform>
                .NewConfig()
                .Map(dest => dest.ExternalId, src => src.Id)
                .Map(dest => dest.Name, src => src.Name);

            TypeAdapterConfig<PlatformCreateDto, Platform>
                .NewConfig()
                .Map(dest => dest.ExternalId, src => src.Id)
                .Map(dest => dest.Name, src => src.Name);
        }
    }
}