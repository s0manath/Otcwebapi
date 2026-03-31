using Microsoft.AspNetCore.Mvc;
using OTC.Api.Models;
using OTC.Api.Services;

namespace OTC.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoginMasterController : ControllerBase
{
    private readonly ILoginMasterService _loginMasterService;

    public LoginMasterController(ILoginMasterService loginMasterService)
    {
        _loginMasterService = loginMasterService;
    }

    [HttpPost("search")]
    public async Task<IActionResult> Search([FromBody] LoginMasterSearchRequest request)
    {
        var result = await _loginMasterService.SearchLoginsAsync(request);
        return Ok(result);
    }

    [HttpPost("detail")]
    public async Task<IActionResult> Get([FromBody] StringIdRequest request)
    {
        var result = await _loginMasterService.GetLoginByIdAsync(request.Id);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPost("save")]
    public async Task<IActionResult> Save([FromBody] LoginMasterRequest request)
    {
        var result = await _loginMasterService.SaveLoginAsync(request);
        return Ok(result);
    }

    [HttpPost("hierarchy")]
    public async Task<IActionResult> GetHierarchy([FromBody] HierarchyRequest request)
    {
        var result = await _loginMasterService.GetHierarchyAsync(request.Type, request.ParentId);
        return Ok(result);
    }
}
