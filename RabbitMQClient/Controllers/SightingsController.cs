using Microsoft.AspNetCore.Mvc;
using RabbitMQClient.Services;

namespace RabbitMQClient.Controllers;

[ApiController]
[Route("[controller]")]
public class SightingsController : ControllerBase
{
    private readonly ILogger<SightingsController> _logger;

    public SightingsController(ILogger<SightingsController> logger, ISightingsService _sightingsService)
    {
        _logger = logger;
    }
    [HttpGet(Name = "active-visitors")]
    public int Get()
    {
        return 3;
    }

   
}