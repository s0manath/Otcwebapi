namespace OTC.Api.Models
{
    public class ZomMaster
    {
        public string Id { get; set; }
        public string ZomCode { get; set; }
        public string ZomName { get; set; }
        public string? RegionId { get; set; }
        public string RegionName { get; set; }
        public string ? LocationId { get; set; }
        public string LocationName { get; set; }
        public string EmailId { get; set; }
        public string MobileNumber { get; set; }
        public bool IsActive { get; set; }
    }
}
