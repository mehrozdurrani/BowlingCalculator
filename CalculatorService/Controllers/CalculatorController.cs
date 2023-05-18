using Microsoft.AspNetCore.Mvc;
using CalculatorFunctions;

namespace CalculatorService.Controllers;

[ApiController]
[Route("[controller]")]
public class CalculatorController : ControllerBase
{
    private readonly ILogger<CalculatorController> _logger;

    // Dependency Injection
    private readonly ICalculator _calculator;

    public CalculatorController(ILogger<CalculatorController> logger, ICalculator calculator)
    {
        _logger = logger;
        _calculator = calculator;

        // Starting Calculator Service
        _calculator.StartListening();
    }

    [HttpGet(Name = "calculate")]
    public int Get()
    {
        /*
         Using simple console application instead of API
         should have been an better approach. It serves the
         pupose so I decided to keep it this way.
         */
        return 0;
    }
}

