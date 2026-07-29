using Mapster;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Data;
using PlatformService.SyncDataService.Http;

namespace PlatformService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlatformsController(IPlatformRepo platformRepo, ICommandDataClient commandDataClient) : ControllerBase
    {
        private readonly IPlatformRepo _platformRepo = platformRepo;
        private readonly ICommandDataClient _commandDataClient = commandDataClient;

        [HttpGet]
        public ActionResult<IEnumerable<Models.PlatformReadDto>> Get()
        {
            var platforms = _platformRepo.GetAllPlatforms();
            return Ok(platforms.Adapt<List<Models.PlatformReadDto>>());
        }

         [HttpGet("{id}")]
        public ActionResult<Models.PlatformReadDto> Get(int id)
        {
            var platform = _platformRepo.GetPlatformById(id);
            if (platform == null)
            {
                return NotFound();
            }
            return Ok(platform.Adapt<Models.PlatformReadDto>());
        }

        [HttpPost]
        public async Task<ActionResult<Models.PlatformReadDto>> Post([FromBody] Models.PlatformCreateDto platformDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var platform = platformDto.Adapt<Models.Platform>();
            _platformRepo.CreatePlatform(platform);
            _platformRepo.SaveChanges();

            try
            {
                await _commandDataClient.SendPlatformToCommand(platform.Adapt<Models.PlatformReadDto>());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not send synchronously: {ex.Message}");
            }
           
            return CreatedAtAction(nameof(Get), new { id = platform.Id }, platform.Adapt<Models.PlatformReadDto>());
        }
    }
}