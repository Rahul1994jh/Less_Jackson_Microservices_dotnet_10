using Grpc.Core;
using Mapster;
using PlatformService.Data;

namespace PlatformService.SyncDataService.Grpc
{
    public sealed class GrpcPlatformService(IPlatformRepo platformRepo) : GrpcPlatform.GrpcPlatformBase
    {
        private readonly IPlatformRepo platformRepo = platformRepo;

        public Task<PlatformsResponse> GetPlatformsAsync(GetAllRequest request, ServerCallContext context)
        {
            var platforms = platformRepo.GetAllPlatforms();
            var response = new PlatformsResponse();
            response.Platforms.AddRange(platforms.Select(p => p.Adapt<GrpcPlatformModel>()));
            return Task.FromResult(response);
        }
    }
}
