using System;
using System.Collections.Generic;

namespace OTC.Api.Models
{
    public class LocationMaster
    {
        public int Id { get; set; }
        public string LocationName { get; set; } = string.Empty;
        public string RegionCode { get; set; } = string.Empty;
        public string RegionName { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }

    public class RegionMaster
    {
        public int Id { get; set; }
        public string RegionCode { get; set; } = string.Empty;
        public string RegionName { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }

    public class KeyInventoryMaster
    {
        public int Id { get; set; }
        public string KeySerialNumber { get; set; } = string.Empty;
        public string KeyType { get; set; } = string.Empty;
        public string KeyMake { get; set; } = string.Empty;
        public string KeyModel { get; set; } = string.Empty;
        public string ATMID { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }

    public class OneLineMaster
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ContactNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }

    public class SiteAccessMaster
    {
        public int Id { get; set; }
        public string SiteID { get; set; } = string.Empty;
        public string SiteName { get; set; } = string.Empty;
        public string AccessTimeFrom { get; set; } = string.Empty;
        public string AccessTimeTo { get; set; } = string.Empty;
        public Dictionary<string, bool> AvailableDays { get; set; } = new();
        public bool IsActive { get; set; } = true;
    }

    public class RouteMasterAdmin
    {
        public int Id { get; set; }
        public string RouteKey { get; set; } = string.Empty;
        public string CustodianId { get; set; } = string.Empty;
        public string TouchKeyId { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string ZOM { get; set; } = string.Empty;
        public string Franchise { get; set; } = string.Empty;
        public string MobileNumber { get; set; } = string.Empty;
        public string CroType { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }

    public class AdminMasterSearchRequest
    {
        public string? Query { get; set; }
        public bool? IsActive { get; set; }
    }
}
