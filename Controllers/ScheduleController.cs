using Microsoft.AspNetCore.Mvc;
using OTC.Api.Services;
using OTC.Api.Models;

namespace OTC.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ScheduleController : ControllerBase
{
    private readonly IScheduleService _scheduleService;

    public ScheduleController(IScheduleService scheduleService)
    {
        _scheduleService = scheduleService;
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetList([FromQuery] string fromDate, [FromQuery] string toDate, [FromQuery] string username = "admin", [FromQuery] string? searchField = null, [FromQuery] string? startWith = null)
    {
        try
        {
            var data = await _scheduleService.GetScheduleListAsync(fromDate, toDate, username, searchField, startWith);
            return Ok(data);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Insert([FromBody] ScheduleInsertRequest request)
    {
        try
        {
            var success = await _scheduleService.InsertScheduleAsync(request);
            if (success) return Ok(new { message = "Scheduled successfully" });
            return BadRequest(new { message = "Failed to schedule" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] ScheduleUpdateRequest request)
    {
        try
        {
            request.ScheduleId = id;
            var success = await _scheduleService.UpdateScheduleAsync(request);
            if (success) return Ok(new { message = "Updated successfully" });
            return BadRequest(new { message = "Failed to update" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id, [FromQuery] string username = "admin")
    {
        try
        {
            var success = await _scheduleService.DeleteScheduleAsync(id, username);
            if (success) return Ok(new { message = "Deleted successfully" });
            return BadRequest(new { message = "Failed to delete. It might be already routed." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet("activity-types")]
    public async Task<IActionResult> GetActivityTypes()
    {
        try
        {
            var data = await _scheduleService.GetActivityTypesAsync();
            return Ok(data);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }
}
