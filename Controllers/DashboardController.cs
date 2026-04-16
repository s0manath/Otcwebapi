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

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary([FromQuery] string date, [FromQuery] string username = "admin")
    {
        try
        {
            var data = await _dashboardService.GetSummaryMetricsAsync(date, username);
            return Ok(data);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet("chart")]
    public async Task<IActionResult> GetChartData([FromQuery] string date, [FromQuery] string username = "admin")
    {
        try
        {
            var data = await _dashboardService.GetChartDataAsync(date, username);
            return Ok(data);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet("district-report")]
    public async Task<IActionResult> GetDistrictReport([FromQuery] string date, [FromQuery] string username = "admin")
    {
        try
        {
            var data = await _dashboardService.GetDistrictReportAsync(date, username);
            return Ok(data);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }
}
