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

    [HttpPost("list")]
    public async Task<IActionResult> GetList([FromBody] ScheduleListRequest request)
    {
        try
        {
            var data = await _scheduleService.GetScheduleListAsync(request.FromDate, request.ToDate, request.Username, request.SearchField, request.StartWith);
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

    [HttpPost("update")]
    public async Task<IActionResult> Update([FromBody] ScheduleUpdateRequest request)
    {
        try
        {
            var success = await _scheduleService.UpdateScheduleAsync(request);
            if (success) return Ok(new { message = "Updated successfully" });
            return BadRequest(new { message = "Failed to update" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpPost("delete")]
    public async Task<IActionResult> Delete([FromBody] ScheduleDeleteRequest request)
    {
        try
        {
            var success = await _scheduleService.DeleteScheduleAsync(request.Id, request.Username);
            if (success) return Ok(new { message = "Deleted successfully" });
            return BadRequest(new { message = "Failed to delete. It might be already routed." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpPost("activity-types")]
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
