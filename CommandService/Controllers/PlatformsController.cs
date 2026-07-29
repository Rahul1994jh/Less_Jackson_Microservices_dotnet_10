using Microsoft.AspNetCore.Mvc;

namespace CommandsServices.Controllers
{
    [ApiController]
    [Route("api/command/[controller]")]
    public class PlatformsController : ControllerBase
    {
        [HttpPost]
        public ActionResult Post()
        {
            Console.WriteLine("--> Inbound Post # Command Service"); 
            return Ok();
        }
    }
}
