using System.Collections.Generic;
using System.Threading.Tasks;
using OTC.Api.Models;

namespace OTC.Api.Services
{
    public interface IMasterService
    {
        // Custodian Master
        Task<IEnumerable<CustodianMaster>> GetCustodiansAsync(string field = null, string startsWith = null, string chklocked = null, string userName = null);
        Task<CustodianMaster?> GetCustodianByIdAsync(int id);
        Task<string> SaveCustodianAsync(CustodianMaster custodian, string userName);

        // Franchise Master
        Task<IEnumerable<FranchiseMaster>> GetFranchisesAsync(string filterField = null, string filterValue = null);
        Task<FranchiseMaster?> GetFranchiseByIdAsync(int id);
        Task<string> SaveFranchiseAsync(FranchiseMaster franchise, string userName);

        // ATM Master
        Task<IEnumerable<AtmMaster>> GetAtmsAsync();
        Task<AtmMaster?> GetAtmByIdAsync(string atmId);
        Task<string> SaveAtmAsync(AtmMaster atm, string userName);

        // Dropdowns
        Task<IEnumerable<MasterDropdownItem>> GetLocationsAsync();
        Task<IEnumerable<MasterDropdownItem>> GetZomsAsync();
        Task<IEnumerable<MasterDropdownItem>> GetFranchiseDropdownAsync();
        Task<IEnumerable<MasterDropdownItem>> GetRouteKeysAsync();
        Task<IEnumerable<MasterDropdownItem>> GetStatesAsync();
        Task<IEnumerable<MasterDropdownItem>> GetDistrictsByStateAsync(int stateId);
        Task<IEnumerable<MasterDropdownItem>> GetCustodiansDropdownAsync();

        // State & District Management
        Task<IEnumerable<StateMaster>> GetStatesAllAsync(string stateName = null);
        Task<StateMaster?> GetStateByIdAsync(int id);
        Task<string> SaveStateAsync(StateMaster state, string userName);
        
        Task<IEnumerable<DistrictMaster>> GetDistrictsAllAsync(string districtName = null, int? stateId = null);
        Task<DistrictMaster?> GetDistrictByIdAsync(int id);
        Task<string> SaveDistrictAsync(DistrictMaster district, string userName);
        Task<IEnumerable<MasterDropdownItem>> GetRegionsAsync();
    }
}
