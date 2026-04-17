using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
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

        public async Task<CustodianMaster?> GetCustodianByIdAsync(int id)
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
            parameters.Add("@IEMI_No", custodian.IemiNo);
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

        public async Task<FranchiseMaster?> GetFranchiseByIdAsync(int id)
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
            string sql = "SELECT top 100 EquipId as AtmId, Aliasatmid as AliasAtmId, Bank, SiteID, Site, City, State, AtmStatus, FranchiseCode as Franchise FROM Purchase";
            return await connection.QueryAsync<AtmMaster>(sql);
        }

        public async Task<AtmMaster?> GetAtmByIdAsync(string atmId)
        {
            using var connection = new SqlConnection(_connectionString);
            var parameters = new DynamicParameters();
            parameters.Add("@EquipId", atmId);
            parameters.Add("@CustodianInfo", dbType: DbType.String, direction: ParameterDirection.Output, size: -1);

            var atm = await connection.QueryFirstOrDefaultAsync<AtmMaster>("usp_FillATMDetails", parameters, commandType: CommandType.StoredProcedure);
            
            if (atm != null)
            {
                // Fetch mapped custodians from mapping table
                var mappings = await connection.QueryAsync<string>("SELECT CustodianCode FROM CustodianMapping WHERE EquipId = @atmId", new { atmId });
                var mappingList = mappings.AsList();
                if (mappingList.Count > 0) atm.Custodian1 = mappingList[0];
                if (mappingList.Count > 1) atm.Custodian2 = mappingList[1];
                if (mappingList.Count > 2) atm.Custodian3 = mappingList[2];
            }

            return atm;
        }

        public async Task<string> SaveAtmAsync(AtmMaster atm, string userName)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

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
            parameters.Add("@Custodian", DBNull.Value);
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
            parameters.Add("@Utype", "Admin");
            parameters.Add("@Ucode", userName);
            parameters.Add("@geotag", "0");
            parameters.Add("@RouteKey", atm.RouteKey);
            parameters.Add("@CROType", "0");
            parameters.Add("@Bulkzom", "0");

            // Logic to check if update or insert
            bool exists = await connection.ExecuteScalarAsync<bool>("SELECT COUNT(1) FROM Purchase WHERE EquipId = @atmId", new { atmId = atm.AtmId }, transaction);
            parameters.Add("@Module", exists ? "Update" : "Insert");

            try
            {
                // 1. Save core ATM data
                await connection.ExecuteAsync("sp_ATMmastV1", parameters, transaction, commandType: CommandType.StoredProcedure);

                // 2. Handle Custodian Mapping
                if (exists)
                {
                    // Archive existing mappings to history
                    await connection.ExecuteAsync("INSERT INTO CustodianMapping_His SELECT Equipid, CustodianCode, GETDATE(), @userName FROM CustodianMapping WHERE EquipId = @atmId", new { atmId = atm.AtmId, userName }, transaction);
                    // Clear existing mappings
                    await connection.ExecuteAsync("DELETE FROM CustodianMapping WHERE EquipId = @atmId", new { atmId = atm.AtmId }, transaction);
                }

                // Insert new mappings
                var custodians = new List<string> { atm.Custodian1, atm.Custodian2, atm.Custodian3 };
                foreach (var custodianCode in custodians)
                {
                    if (!string.IsNullOrEmpty(custodianCode))
                    {
                        await connection.ExecuteAsync("INSERT INTO CustodianMapping (EquipId, CustodianCode) VALUES (@atmId, @custodianCode)", new { atmId = atm.AtmId, custodianCode }, transaction);
                    }
                }

                await transaction.CommitAsync();
                return string.Empty;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
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
            return await connection.QueryAsync<MasterDropdownItem>("select slno as Id, state_name as Name from StateMaster order by state_name");
        }

        public async Task<IEnumerable<MasterDropdownItem>> GetCustodiansDropdownAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<MasterDropdownItem>("SELECT CustodianCode as Id, CustodianName as Name FROM CustodianMaster ORDER BY CustodianName");
        }

        #endregion

        #region State & District Management

        public async Task<IEnumerable<StateMaster>> GetStatesAllAsync(string stateName = null)
        {
            using var connection = new SqlConnection(_connectionString);
            var parameters = new DynamicParameters();
            parameters.Add("@StateName", string.IsNullOrEmpty(stateName) ? (object)DBNull.Value : stateName);
            
            // Legacy GetStateMaster SP
            return await connection.QueryAsync<StateMaster>("GetStateMaster", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<StateMaster?> GetStateByIdAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            var parameters = new DynamicParameters();
            parameters.Add("@stateId", id);
            
            return await connection.QueryFirstOrDefaultAsync<StateMaster>("GetStateMasterById", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<string> SaveStateAsync(StateMaster state, string userName)
        {
            using var connection = new SqlConnection(_connectionString);
            var parameters = new DynamicParameters();
            
            bool isUpdate = state.Id > 0;
            string spName = isUpdate ? "Proc_UpdateState" : "Proc_InsertState";
            
            if (isUpdate)
            {
                parameters.Add("@slno", state.Id);
            }
            parameters.Add("@StateName", state.StateName);
            parameters.Add("@roCode", state.RegionCode);

            try
            {
                if (isUpdate)
                {
                    var message = await connection.ExecuteScalarAsync<string>(spName, parameters, commandType: CommandType.StoredProcedure);
                    if (message != null && (message.Contains("Successfully") || message.Contains("Updated"))) return string.Empty;
                    return message ?? "Update failed without error message.";
                }
                else
                {
                    await connection.ExecuteAsync(spName, parameters, commandType: CommandType.StoredProcedure);
                    return string.Empty;
                }
            }
            catch (SqlException ex)
            {
                return ex.Message;
            }
        }

        public async Task<IEnumerable<DistrictMaster>> GetDistrictsAllAsync(string districtName = null, int? stateId = null)
        {
            using var connection = new SqlConnection(_connectionString);
            var parameters = new DynamicParameters();
            parameters.Add("@district_name", string.IsNullOrEmpty(districtName) ? (object)DBNull.Value : districtName);
            parameters.Add("@state_id", stateId ?? 0);
            
            return await connection.QueryAsync<DistrictMaster>("Proc_GetDistricts", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<DistrictMaster?> GetDistrictByIdAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            var parameters = new DynamicParameters();
            parameters.Add("@districtSlno", id);
            
            return await connection.QueryFirstOrDefaultAsync<DistrictMaster>("Proc_GetDistrictById", parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<string> SaveDistrictAsync(DistrictMaster district, string userName)
        {
            using var connection = new SqlConnection(_connectionString);
            var parameters = new DynamicParameters();
            
            bool isUpdate = district.Id > 0;
            string spName = isUpdate ? "Proc_UpdateDistrict" : "Proc_InsertDistrict";
            
            if (isUpdate)
            {
                parameters.Add("@districtSlNo", district.Id);
            }
            parameters.Add("@district_name", district.DistrictName);
            parameters.Add("@state_id", district.StateId);
            parameters.Add("@created_by", userName);

            try
            {
                var message = await connection.ExecuteScalarAsync<string>(spName, parameters, commandType: CommandType.StoredProcedure);
                if (message != null && (message.Contains("Successfully") || message.Contains("Updated") || message.Contains("Inserted"))) return string.Empty;
                return message ?? "Operation failed without error message.";
            }
            catch (SqlException ex)
            {
                return ex.Message;
            }
        }

        public async Task<IEnumerable<MasterDropdownItem>> GetRegionsAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<MasterDropdownItem>("Sp_GetRegion", commandType: CommandType.StoredProcedure);
        }

        #endregion
    }
}
