using Microsoft.AspNetCore.Mvc;
using RollsFunctions;
using FrameClass;

namespace RollsService.Controllers;

[ApiController]
[Route("[controller]")]
public class RollsController : ControllerBase
{
    private readonly ILogger<RollsController> _logger;
    private readonly IRolls _rolls;

    // Frames for testing
    //Frames[] frames = new Frames[]
    //{
    //new Frames { roll1 = 1, roll2 = 2 },
    //new Frames { roll1 = 4, roll2 = 5 },
    //new Frames { roll1 = 7, roll2 = 3 }
    //};

    public RollsController(ILogger<RollsController> logger, IRolls rolls)
    {
        _logger = logger;
        _rolls = rolls;
    }

    [HttpPost]
    public IActionResult PostRolls([FromBody] Frames[] frames)
    {
        if (frames == null)
        {
            return BadRequest("Invalid Frames data");
        }
        else
        _rolls.SendRolls(frames);

        return Ok("Frames Recieved Successfully");
    }
}

