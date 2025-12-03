using System.Security.Claims;
// Authorization removed for local testing
using Microsoft.AspNetCore.Mvc;
using ProductExpirationTracker.Application.Interfaces;

namespace ProductExpirationTracker.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StatsController : ControllerBase
{
    private readonly IStatsService _statsService;
    public StatsController(IStatsService statsService) => _statsService = statsService;

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string? from, [FromQuery] string? to, [FromQuery] string? granularity)
    {
        // Authorization completely disabled for local testing — do not require user claim.
        // Parse dates safely; if parsing fails, return BadRequest.
        DateTime toDate;
        if (string.IsNullOrWhiteSpace(to))
            toDate = DateTime.UtcNow.Date;
        else if (!DateTime.TryParse(to, out toDate))
            return BadRequest("Invalid 'to' date format. Use YYYY-MM-DD.");

        DateTime fromDate;
        if (string.IsNullOrWhiteSpace(from))
            fromDate = toDate.AddMonths(-5).AddDays(1 - toDate.Day); // first day of 6-month window
        else if (!DateTime.TryParse(from, out fromDate))
            return BadRequest("Invalid 'from' date format. Use YYYY-MM-DD.");

        var stats = await _statsService.GetStatsAsync(null, fromDate.Date, toDate.Date, granularity ?? "month");
        return Ok(stats);
    }
}
