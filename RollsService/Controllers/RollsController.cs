using Microsoft.AspNetCore.Mvc;
using RollsFunctions;
using FrameClass;

namespace RollsService.Controllers;

[ApiController]
[Route("[controller]")]
public class RollsController : ControllerBase
{
    private readonly ILogger<RollsController> _logger;

    // Dependency Injection
    private readonly IRolls _rolls;


    public RollsController(ILogger<RollsController> logger, IRolls rolls)
    {
        _logger = logger;
        _rolls = rolls;
    }

    // POST function to Post Frames of Rolls
    [HttpPost]
    public IActionResult PostRolls([FromBody] Frames[] framesOfRolls)
    {
        if (framesOfRolls == null)
        {
            return BadRequest("Invalid Frames data");
        }
        else
        _rolls.SendRolls(framesOfRolls);

        return Ok("Frames Recieved Successfully");
    }
}

