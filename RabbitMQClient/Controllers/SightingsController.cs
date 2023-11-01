using Microsoft.AspNetCore.Mvc;
using RabbitMQClient.Services;

namespace RabbitMQClient.Controllers;

[ApiController]
[Route("[controller]")]
public class SightingsController : ControllerBase
{
    private readonly ILogger<SightingsController> _logger;
    private readonly ISightingsService _sightingsService;

    public SightingsController(ILogger<SightingsController> logger, ISightingsService sightingsService)
    {
        _logger = logger;
        _sightingsService = sightingsService;
    }
    [HttpGet(Name = "active-visitors")]
    public string Get(string[]? cameras = null, string day ="")
    {
        return _sightingsService.GetActiveVisitors( "2023/5/12", "64354cb3da408e8ff468a238");
    }

   
}