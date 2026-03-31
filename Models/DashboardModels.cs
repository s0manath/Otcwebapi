namespace OTC.Api.Models;

public class DashboardStats
{
    public string Label { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Change { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
}

public class DistrictPerformance
{
    public string DistrictName { get; set; } = string.Empty;
    public string DistrictCode { get; set; } = string.Empty;
    public int Total { get; set; }
    public int Completed { get; set; }
    public int Reset { get; set; }
    public int Pending { get; set; }
}

public class DashboardData
{
    public List<DashboardStats> Stats { get; set; } = new();
    public List<DistrictPerformance> Performance { get; set; } = new();
}
