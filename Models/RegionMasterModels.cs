namespace OTC.Api.Models
{
    public class RegionMaster
    {
        public string RegionCode { get; set; } = string.Empty;
        public string RegionName { get; set; } = string.Empty;
        public string RegionEmail {  get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
}
