using Microsoft.AspNetCore.Mvc;
using OTC.Api.Services;

namespace OTC.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet("data")]
    public async Task<IActionResult> GetDashboardData([FromQuery] string date, [FromQuery] string username = "admin")
    {
        try
        {
            var data = await _dashboardService.GetDashboardDataAsync(date, username);
            return Ok(data);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }
}
