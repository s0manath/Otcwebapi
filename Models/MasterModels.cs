using System;

namespace OTC.Api.Models
{
    public class CustodianMaster
    {
        public int Id { get; set; }
        public string CustodianName { get; set; }
        public string MobileNumber { get; set; }
        public string EmailId { get; set; }
        public int? LocationId { get; set; }
        public string LocationName { get; set; }
        public int? ZomId { get; set; }
        public string ZomName { get; set; }
        public int? FranchiseId { get; set; }
        public string FranchiseName { get; set; }
        public int? RouteKeyId { get; set; }
        public string RouteKeyName { get; set; }
        public string TouchKeyId { get; set; }
        public string CustodianCode { get; set; } // txtcid in legacy
        public DateTime? AccessFrom { get; set; }
        public DateTime? AccessTo { get; set; }
        public string IemiNo { get; set; }
        public string ProfileImage { get; set; }
        public bool IsActive { get; set; }
    }

    public class FranchiseMaster
    {
        public int Id { get; set; }
        public string FranchiseName { get; set; }
        public string MobileNumber { get; set; }
        public string EmailId { get; set; }
        public string SapCode { get; set; }
        public bool SecondaryCustodianRequire { get; set; }
        public int? StateId { get; set; }
        public string StateName { get; set; }
        public int? DistrictId { get; set; }
        public string DistrictName { get; set; }
        public bool IsActive { get; set; }
    }

    public class AtmMaster
    {
        public string AtmId { get; set; }
        public string AliasAtmId { get; set; }
        public string Bank { get; set; }
        public string SiteId { get; set; }
        public string Site { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Pincode { get; set; }
        public string Region { get; set; }
        public string Location { get; set; }
        public string? InstallDate { get; set; }
        public string AtmCategory { get; set; }
        public string Model { get; set; }
        public string LoiCode { get; set; }
        public string KeyNumber { get; set; }
        public string SerialNo { get; set; }
        public string Comments { get; set; }
        public string AtmStatus { get; set; }
        public string AtmType { get; set; }
        public string Franchise { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Zom { get; set; }
        public string Custodian1 { get; set; }
        public string Custodian2 { get; set; }
        public string Custodian3 { get; set; }
        public string RouteKey { get; set; }
        public bool IsActive { get; set; }
    }


    public class MasterDropdownItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
