using PlatformService.Models;

namespace PlatformService.Data
{
    public class PlatformRepo(AppDbContext context) : IPlatformRepo
    {
        private readonly AppDbContext _context = context;

        public bool SaveChanges() => _context.SaveChanges() >= 0;

        public IEnumerable<Platform> GetAllPlatforms() => [.. _context.Platforms];

        public Platform? GetPlatformById(int id) => _context.Platforms.FirstOrDefault(p => p.Id == id);

        public void CreatePlatform(Platform platform) =>_context.Platforms.Add(platform);

        public void DeletePlatform(int id)
        {
            var platform = _context.Platforms.FirstOrDefault(p => p.Id == id);
            if (platform != null)
            {
                _context.Platforms.Remove(platform);
            }
        } 
    }
}