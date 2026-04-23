using System;
using System.Collections.Generic;

namespace OTC.Api.Models
{
    public class RoleMaster
    {
        public long SlNo { get; set; }
        public string RoleName { get; set; }
        public string RoleDescription { get; set; }
        public int? RoleStatus { get; set; } // 1: Active, 0: Inactive
        public int? RoleDepartment { get; set; }
        public int? CoustodianNoneAvailable { get; set; }
        public List<ModuleAccess> Privileges { get; set; } = new List<ModuleAccess>();
        public List<ReportAccess> ReportPrivileges { get; set; } = new List<ReportAccess>();
        public DateTime? CreatedOn { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public int? UpdatedBy { get; set; }
    }

    public class ModuleAccess
    {
        public string ModuleName { get; set; }
        public bool Add { get; set; }
        public bool Edit { get; set; }
        public bool View { get; set; }
        public bool Delete { get; set; }
    }

    public class ReportAccess
    {
        public string ReportName { get; set; }
        public bool View { get; set; }
    }

    public class RoleSearchRequest
    {
        public string RoleName { get; set; }
        public int? Status { get; set; }
    }
}
