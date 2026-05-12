namespace OTC.Api.Models
{
  
    public class CustodianMaster
    {
        public string? Id { get; set; }
        public string? CustodianName { get; set; }
        public string? MobileNumber { get; set; }
        public string? EmailId { get; set; }
        public string? LocationId { get; set; }
        public string? LocationName { get; set; }
        public string? ZomId { get; set; }
        public string? ZomName { get; set; }
        public string? FranchiseId { get; set; }
        public string? FranchiseName { get; set; }
        public string? RouteKeyId { get; set; }
        public string? RouteKeyName { get; set; }
        public string? TouchKeyId { get; set; }
        public string? CustodianCode { get; set; } 
        public DateTime? AccessFrom { get; set; }
        public DateTime? AccessTo { get; set; }
        public string? IemiNo { get; set; }
        public string? ProfileImage { get; set; }
        public string? IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
    }

    public class CustodianMasterRequest
    {   
            public string? Field { get; set; }
            public string? StartsWith { get; set; }
            public string? ChkLocked { get; set; }
            public string? UserName { get; set; }


    }
}
