namespace OTC.Api.Models
{
    

    public class LocationMaster
    {
        public int Id { get; set; }

        public string LocationCode { get; set; } = string.Empty;
        public string LocationName { get; set; } = string.Empty;
        public string RegionCode { get; set; } = string.Empty;
        public string RegionName { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;

    }
}
