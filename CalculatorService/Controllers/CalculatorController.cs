using Microsoft.AspNetCore.Mvc;
using CalculatorFunctions;

namespace CalculatorService.Controllers;

[ApiController]
[Route("[controller]")]
public class CalculatorController : ControllerBase
{
    private readonly ILogger<CalculatorController> _logger;
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

        return 0;
    }
}

