using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using OTC.Api.Models;

namespace OTC.Api.Services
{
    public class MasterService : IMasterService
    {
        private readonly string _connectionString;

        public MasterService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new Exception("Connection string 'DefaultConnection' not found.");
        }

        #region Custodian Master

        public async Task<IEnumerable<CustodianMaster>> GetCustodiansAsync(string field = null, string startsWith = null, string chklocked = null, string userName = null)
        {
            using var connection = new SqlConnection(_connectionString);
            var parameters = new DynamicParameters();
            parameters.Add("@Field", string.IsNullOrEmpty(field) || field == "None" ? DBNull.Value : field);
            parameters.Add("@StartsWith", string.IsNullOrEmpty(startsWith) ? DBNull.Value : startsWith);
            parameters.Add("@chklocked", string.IsNullOrEmpty(chklocked) ? DBNull.Value : chklocked);
            parameters.Add("@UserName", string.IsNullOrEmpty(userName) ? DBNull.Value : userName);

            return await connection.QueryAsync<CustodianMaster>("usp_GetCustodianData", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<CustodianMaster> GetCustodianByIdAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            var parameters = new DynamicParameters();
            parameters.Add("@CustodianCode", id);

            var dt = await connection.QueryFirstOrDefaultAsync<CustodianMaster>("usp_CustodianMaster_GetByCode", parameters, commandType: CommandType.StoredProcedure);
            return dt;
        }

        public async Task<string> SaveCustodianAsync(CustodianMaster custodian, string userName)
        {
            using var connection = new SqlConnection(_connectionString);
            var parameters = new DynamicParameters();
            
            bool isUpdate = custodian.Id > 0;
            string spName = isUpdate ? "usp_CustodianMaster_Update" : "usp_CustodianMaster_Insert";

            if (isUpdate)
            {
                parameters.Add("@CustodianCode", custodian.Id);
                parameters.Add("@Custodian_modifiedBy", userName);
            }
            else
            {
                parameters.Add("@Photo", custodian.ProfileImage);
                parameters.Add("@Custodian_CreatedBy", userName);
            }

            parameters.Add("@CustodianName", custodian.CustodianName);
            parameters.Add("@MobileNo", custodian.MobileNumber);
            parameters.Add("@EmailID", custodian.EmailId);
            parameters.Add("@LocationName", custodian.LocationId);
            parameters.Add("@FranschiseName", custodian.FranchiseId);
            parameters.Add("@IEMI_No", custodian.TouchKeyId); // txtIEMI_No in legacy
            parameters.Add("@AccessFrom", custodian.AccessFrom);
            parameters.Add("@AccessTo", custodian.AccessTo);
            parameters.Add("@Zomname", custodian.ZomId);
            parameters.Add("@TouchKeyID", custodian.TouchKeyId);
            parameters.Add("@CustodianID", custodian.CustodianCode);

            try
            {
                await connection.ExecuteAsync(spName, parameters, commandType: CommandType.StoredProcedure);
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        #endregion

        #region Franchise Master

        public async Task<IEnumerable<FranchiseMaster>> GetFranchisesAsync(string filterField = null, string filterValue = null)
        {
            using var connection = new SqlConnection(_connectionString);
            var parameters = new DynamicParameters();
            parameters.Add("@FilterField", string.IsNullOrEmpty(filterField) || filterField == "None" ? DBNull.Value : filterField);
            parameters.Add("@FilterValue", string.IsNullOrEmpty(filterValue) ? DBNull.Value : filterValue);

            return await connection.QueryAsync<FranchiseMaster>("Proc_GetFranchiseMasterWithFilter", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<FranchiseMaster> GetFranchiseByIdAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            var parameters = new DynamicParameters();
            parameters.Add("@FranchiseCode", id);

            return await connection.QueryFirstOrDefaultAsync<FranchiseMaster>("Proc_GetFranchiseDetailsByFranchiseCode", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<string> SaveFranchiseAsync(FranchiseMaster franchise, string userName)
        {
            using var connection = new SqlConnection(_connectionString);
            var parameters = new DynamicParameters();
            
            parameters.Add("@Franchise_Code", franchise.Id > 0 ? (object)franchise.Id : DBNull.Value);
            parameters.Add("@Franchise_Name", franchise.FranchiseName);
            parameters.Add("@Franchise_Email", franchise.EmailId);
            parameters.Add("@Franchise_mobile", franchise.MobileNumber);
            parameters.Add("@Franchise_Sapcode", franchise.SapCode);
            parameters.Add("@Franchise_secondchk", franchise.SecondaryCustodianRequire ? "1" : "0");
            parameters.Add("@CreatedBy", userName);
            parameters.Add("@ModifiedBy", userName);
            parameters.Add("@state", franchise.StateId);
            parameters.Add("@district", franchise.DistrictId);
            parameters.Add("@Module", franchise.Id > 0 ? "Update" : "Insert");

            try
            {
                await connection.ExecuteAsync("sp_FranchiseMast", parameters, commandType: CommandType.StoredProcedure);
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        #endregion

        #region ATM Master

        public async Task<IEnumerable<AtmMaster>> GetAtmsAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            // Reusing the simple list query or direct SQL for ATM summary
            string sql = "SELECT EquipId as AtmId, Aliasatmid as AliasAtmId, Bank, SiteID, Site, City, State, AtmStatus, FranchiseCode as Franchise FROM Purchase";
            return await connection.QueryAsync<AtmMaster>(sql);
        }

        public async Task<AtmMaster> GetAtmByIdAsync(string atmId)
        {
            using var connection = new SqlConnection(_connectionString);
            var parameters = new DynamicParameters();
            parameters.Add("@EquipId", atmId);
            parameters.Add("@CustodianInfo", dbType: DbType.String, direction: ParameterDirection.Output, size: -1);

            var atm = await connection.QueryFirstOrDefaultAsync<AtmMaster>("usp_FillATMDetails", parameters, commandType: CommandType.StoredProcedure);
            // Note: CustodianInfo is complex in legacy (delimited string), handle parsing in frontend or here if needed.
            return atm;
        }

        public async Task<string> SaveAtmAsync(AtmMaster atm, string userName)
        {
            using var connection = new SqlConnection(_connectionString);
            var parameters = new DynamicParameters();
            
            parameters.Add("@EquipId", atm.AtmId);
            parameters.Add("@Aliasatmid", atm.AliasAtmId);
            parameters.Add("@Installdate", atm.InstallDate);
            parameters.Add("@Model", atm.Model);
            parameters.Add("@AtmCategory", atm.AtmCategory);
            parameters.Add("@LOIcode", atm.LoiCode);
            parameters.Add("@Keynumber", atm.KeyNumber);
            parameters.Add("@Serial", atm.SerialNo);
            parameters.Add("@Comments", atm.Comments);
            parameters.Add("@Atmstatus", atm.AtmStatus);
            parameters.Add("@Atmtype", atm.AtmType);
            parameters.Add("@Latitude", atm.Latitude);
            parameters.Add("@Longitude", atm.Longitude);
            parameters.Add("@FranchiseCode", atm.Franchise);
            parameters.Add("@ZomCode", atm.Zom);
            parameters.Add("@Custodian", DBNull.Value); // Legacy handles mapping via separate logic usually
            parameters.Add("@Siteid", atm.SiteId);
            parameters.Add("@Bank", atm.Bank);
            parameters.Add("@Site", atm.Site);
            parameters.Add("@Region", atm.Region);
            parameters.Add("@Location", atm.Location);
            parameters.Add("@State", atm.State);
            parameters.Add("@City", atm.City);
            parameters.Add("@PinCode", atm.Pincode);
            parameters.Add("@Address", atm.Address);
            parameters.Add("@CreatedBy", userName);
            parameters.Add("@ModifiedBy", userName);
            parameters.Add("@Utype", "Admin"); // Default
            parameters.Add("@Ucode", userName);
            parameters.Add("@Module", "Insert"); // Logic to check if exists can be added
            parameters.Add("@geotag", "0");
            parameters.Add("@RouteKey", atm.RouteKey);
            parameters.Add("@CROType", "0");
            parameters.Add("@Bulkzom", "0");

            try
            {
                await connection.ExecuteAsync("sp_ATMmastV1", parameters, commandType: CommandType.StoredProcedure);
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        #endregion

        #region Dropdowns

        public async Task<IEnumerable<MasterDropdownItem>> GetLocationsAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<MasterDropdownItem>("sp_FillLocation", commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<MasterDropdownItem>> GetZomsAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<MasterDropdownItem>("Sp_GetZom", commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<MasterDropdownItem>> GetFranchiseDropdownAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<MasterDropdownItem>("Sp_GetFranchise", commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<MasterDropdownItem>> GetRouteKeysAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<MasterDropdownItem>("usp_FillRouteKey", commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<MasterDropdownItem>> GetStatesAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<MasterDropdownItem>("select slno as Id, state_name as Name from State_Master order by state_name");
        }

        public async Task<IEnumerable<MasterDropdownItem>> GetDistrictsByStateAsync(int stateId)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<MasterDropdownItem>("select district_id as Id, district_name as Name from district_master where state_id = @stateId order by district_name", new { stateId });
        }

        #endregion
    }
}
