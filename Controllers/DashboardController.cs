using Microsoft.AspNetCore.Mvc;
using OTC.Api.Models;
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

    [HttpPost("data")]
    public async Task<IActionResult> GetDashboardData([FromBody] DashboardRequest request)
    {
        try
        {
            var data = await _dashboardService.GetDashboardDataAsync(request.Date, request.Username);
            return Ok(data);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }
}
