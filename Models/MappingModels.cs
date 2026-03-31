using System;

namespace OTC.Api.Models
{
    public class CustodianLoginMapping
    {
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty; // Legacy support
        public string CustodianCode { get; set; } = string.Empty;
        public string CustodianEmail { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }

    public class ZomLoginMapping
    {
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty; // Legacy support
        public string ZomCode { get; set; } = string.Empty;
        public string ZomEmail { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }

    public class PendingLoginRequest
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string CustodianOrZomName { get; set; } = string.Empty;
        public DateTime RequestDate { get; set; }
        public string RequestFor { get; set; } = string.Empty; // Login / Password Reset
        public string MobileInfo { get; set; } = string.Empty;
        public string Comments { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected
    }

    public class MappingApprovalRequest
    {
        public int RequestId { get; set; }
        public string Comments { get; set; } = string.Empty;
        public bool IsApproved { get; set; }
    }
}
