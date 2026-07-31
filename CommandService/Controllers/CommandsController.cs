using CommandService.Data;
using CommandService.Models;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace CommandService.Controllers;

[ApiController]
[Route("api/command/platforms/{platformId}/[controller]")]
public class CommandsController(ICommandRepo commandRepo) : ControllerBase
{
    private readonly ICommandRepo commandRepo = commandRepo;

	[HttpGet]
	public ActionResult<IEnumerable<CommandReadDto>> GetCommandsForPlatform(int platformId)
	{
		if (!commandRepo.PlatformExists(platformId))
		{
			return NotFound();
		}

		var command = commandRepo.GetCommandsForPlatform(platformId);

		if (command == null)
		{
			return NotFound();
		}

		return Ok(command.Adapt<IEnumerable<CommandReadDto>>());
	}

    [HttpGet("{commandId}")]
	public ActionResult<IEnumerable<CommandReadDto>> GetCommand(int platformId, int commandId)
	{
		if (!commandRepo.PlatformExists(platformId))
		{
			return NotFound();
		}

		var command = commandRepo.GetCommand(platformId, commandId);

		if (command == null)
		{
			return NotFound();
		}

		return Ok(command.Adapt<CommandReadDto>());
	}
}
