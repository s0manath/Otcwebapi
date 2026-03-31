using Microsoft.AspNetCore.Mvc;
using OTC.Api.Services;
using OTC.Api.Models;

namespace OTC.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RouteController : ControllerBase
{
    private readonly IRouteService _routeService;

    public RouteController(IRouteService routeService)
    {
        _routeService = routeService;
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetList(
        [FromQuery] string? fromDate,
        [FromQuery] string? toDate,
        [FromQuery] string username = "admin",
        [FromQuery] string? region = null,
        [FromQuery] string? district = null,
        [FromQuery] string? franchise = null,
        [FromQuery] string? zom = null,
        [FromQuery] string? activityType = null,
        [FromQuery] string? status = null,
        [FromQuery] string? chkConfig = null,
        [FromQuery] string? searchField = null,
        [FromQuery] string? criteria = null,
        [FromQuery] string? searchValue = null)
    {
        try
        {
            var data = await _routeService.GetRouteListAsync(
                fromDate, toDate, username, region, district, franchise, 
                zom, activityType, status, chkConfig, searchField, criteria, searchValue);
            return Ok(data);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet("filters")]
    public async Task<IActionResult> GetFilters()
    {
        try
        {
            var data = await _routeService.GetFilterOptionsAsync();
            return Ok(data);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet("custodians/{scheduleId}")]
    public async Task<IActionResult> GetCustodians(string scheduleId)
    {
        try
        {
            var data = await _routeService.GetCustodiansAsync(scheduleId);
            return Ok(data);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet("details/{scheduleId}")]
    public async Task<IActionResult> GetDetails(string scheduleId)
    {
        try
        {
            var data = await _routeService.GetRouteDetailsAsync(scheduleId);
            if (data == null) return NotFound();
            return Ok(data);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpPost("save")]
    public async Task<IActionResult> Save([FromBody] RouteSaveRequest request)
    {
        try
        {
            var success = await _routeService.SaveRouteAsync(request);
            if (success) return Ok(new { message = "Route configured successfully" });
            return BadRequest(new { message = "Failed to configure route" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }
}
