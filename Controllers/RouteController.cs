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

    [HttpPost("list")]
    public async Task<IActionResult> GetList([FromBody] RouteListRequest request)
    {
        try
        {
            var data = await _routeService.GetRouteListAsync(
                request.FromDate, request.ToDate, request.Username, request.Region, request.District, request.Franchise, 
                request.Zom, request.ActivityType, request.Status, request.ChkConfig, request.SearchField, request.Criteria, request.SearchValue);
            return Ok(data);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpPost("filters")]
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

    [HttpPost("custodians")]
    public async Task<IActionResult> GetCustodians([FromBody] StringIdRequest request)
    {
        try
        {
            var data = await _routeService.GetCustodiansAsync(request.Id);
            return Ok(data);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpPost("details")]
    public async Task<IActionResult> GetDetails([FromBody] StringIdRequest request)
    {
        try
        {
            var data = await _routeService.GetRouteDetailsAsync(request.Id);
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
