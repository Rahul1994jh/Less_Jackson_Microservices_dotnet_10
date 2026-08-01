using CommandService.Data;
using CommandService.Models;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace CommandService.Controllers
{
    [ApiController]
    [Route("api/command/[controller]")]
    public class PlatformsController(ICommandRepo commandRepo) : ControllerBase
    {
        private readonly ICommandRepo commandRepo = commandRepo;

        [HttpGet]
        public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
        {
            var platforms = commandRepo.GetAllPlatforms();
            return Ok(platforms.Adapt<List<PlatformReadDto>>());
        }

        [HttpPost]
        public ActionResult Post(PlatformCreateDto platformCreateDto)
        {
            commandRepo.CreatePlatform(platformCreateDto.Adapt<Platform>());
            commandRepo.SaveChanges();
            return Ok();
        }
    }
}
