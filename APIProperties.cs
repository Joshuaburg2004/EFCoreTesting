using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api")]
public class ApiController : Controller
{
    private static readonly List<Onderdeel> onderdelen = new OnderdeelContext().Onderdelen.ToList();

    private readonly ILogger<ApiController> _logger;
    private OnderdeelContext _context = new OnderdeelContext();

    public ApiController(ILogger<ApiController> logger)
    {
        _logger = logger;
    }

    [HttpGet("GetOnderdelen")]
    public IEnumerable<Onderdeel> GetOnderdelen()
    {
        return onderdelen;
    }
    [HttpPost]
    public async Task<ActionResult<Onderdeel>> PostOnderdelen(Onderdeel onderdeel)
    {
        _context.Onderdelen.Add(onderdeel);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(onderdeel), new { id = onderdeel.OnderdeelID }, onderdeel);
    }
}