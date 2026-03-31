namespace OTC.Api.Models;

public class ReportRequest
{
    public string? FromDate { get; set; }
    public string? ToDate { get; set; }
    public string? FilterField { get; set; }
    public string? FilterValue { get; set; }
    public string? FranchiseCode { get; set; }
    public string ReportType { get; set; } = string.Empty;
    public string Username { get; set; } = "admin";
}

public class ReportDataResponse
{
    public IEnumerable<string> Columns { get; set; } = new List<string>();
    public IEnumerable<dynamic> Data { get; set; } = new List<dynamic>();
}
