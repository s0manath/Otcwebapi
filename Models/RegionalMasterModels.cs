using System;
using System.Collections.Generic;

namespace OTC.Api.Models
{
    public class StateMaster
    {
        public int Id { get; set; }
        public string StateName { get; set; }
        public int? RegionId { get; set; }
        public string RegionName { get; set; }
        public bool IsActive { get; set; }
    }

    public class DistrictMaster
    {
        public int Id { get; set; }
        public string DistrictName { get; set; }
        public int? StateId { get; set; }
        public string StateName { get; set; }
        public bool IsActive { get; set; }
    }

    public class ZomMaster
    {
        public int Id { get; set; }
        public string ZomName { get; set; }
        public int? RegionId { get; set; }
        public string RegionName { get; set; }
        public int? LocationId { get; set; }
        public string LocationName { get; set; }
        public bool IsActive { get; set; }
    }

    public class RegionalSearchRequest
    {
        public string Name { get; set; }
        public int? ParentId { get; set; }
        public bool? IsActive { get; set; }
    }
}
