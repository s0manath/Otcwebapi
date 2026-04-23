using OTC.Api.Models;

namespace OTC.Api.Services;

public interface IReportService
{
    Task<ReportDataResponse> GetReportDataAsync(ReportRequest request);
    Task StreamReportToZipAsync(ReportRequest request, Stream outputStream);
}
