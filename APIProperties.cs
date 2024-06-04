using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api")]
public class ApiController : Controller
{
    private OnderdeelContext context;
    private readonly ILogger<ApiController> _logger;

    public ApiController(ILogger<ApiController> logger, OnderdeelContext cont)
    {
        _logger = logger; context = cont;
    }

    [HttpGet("getOnderdelen")]
    public IEnumerable<Onderdeel> Get()
    {
        return context.Onderdelen;
    }
}