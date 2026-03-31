using Microsoft.AspNetCore.Mvc;
using OTC.Api.Models;
using OTC.Api.Services;

namespace OTC.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportController(IReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpPost("data")]
    public async Task<IActionResult> GetReportData([FromBody] ReportRequest request)
    {
        try
        {
            var data = await _reportService.GetReportDataAsync(request);
            return Ok(data);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet("franchises")]
    public async Task<IActionResult> GetFranchises()
    {
        try
        {
            var data = await _reportService.GetFranchisesAsync();
            return Ok(data);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }
}
