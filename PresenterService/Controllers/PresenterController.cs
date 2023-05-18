﻿using Microsoft.AspNetCore.Mvc;
using PresenterFunctions;

namespace PresenterService.Controllers;

[ApiController]
[Route("[controller]")]
public class PresenterController : ControllerBase
{
    private readonly ILogger<PresenterController> _logger;
    private readonly IPresenter _presenter;

    public PresenterController(ILogger<PresenterController> logger, IPresenter presenter)
    {
        _logger = logger;
        _presenter = presenter;
    }

    [HttpGet("score")]
    public string Get()
    {
        // Starting Presenter Service
        _presenter.StartPresenterService();
        return _presenter.GetScore();
    }
}

