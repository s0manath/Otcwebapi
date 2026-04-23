using Microsoft.AspNetCore.Mvc;
using OTC.Api.Models;
using OTC.Api.Services;

namespace OTC.Api.Controllers
{
    [ApiController]
    [Route("api/report")]
    public class ReportExportController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportExportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpPost("export-zip")]
        public async Task ExportZip([FromBody] ReportRequest request)
        {
            var fileName = $"{request.ReportType}_{DateTime.Now:yyyyMMdd}.zip";
            
            Response.ContentType = "application/zip";
            Response.Headers.Add("Content-Disposition", $"attachment; filename={fileName}");

            await _reportService.StreamReportToZipAsync(request, Response.Body);
        }
    }
}
