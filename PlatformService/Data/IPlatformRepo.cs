using PlatformService.Models;

namespace PlatformService.Data
{
    public interface IPlatformRepo
    {
        bool SaveChanges();
        Platform? GetPlatformById(int id);
        IEnumerable<Platform> GetAllPlatforms();
        void CreatePlatform(Platform platform);
        void DeletePlatform(int id);
    }
}