using System.Text.Json.Serialization;

namespace OTC.Api.Models;

public class SummaryMetrics
{
    public int Total { get; set; }
    public int Completed { get; set; }
    public int Reset { get; set; }
    public int Pending { get; set; }
}

public class ChartData
{
    [JsonPropertyName("Name")]
    public string Name { get; set; } = string.Empty;
    [JsonPropertyName("Value")]
    public double Value { get; set; }
}

public class DistrictReportItem
{
    [JsonPropertyName("District Name")]
    public string DistrictName { get; set; } = string.Empty;
    [JsonPropertyName("Total")]
    public int Total { get; set; }
    [JsonPropertyName("Completed")]
    public int Completed { get; set; }
    [JsonPropertyName("Reset and Completed")]
    public int ResetAndCompleted { get; set; }
    [JsonPropertyName("Skipped")]
    public int Pending { get; set; }
    [JsonPropertyName("DistrictCode")]
    public string DistrictCode { get; set; } = string.Empty;
}
