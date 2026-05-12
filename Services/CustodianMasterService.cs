using Dapper;
using Microsoft.Data.SqlClient;
using OTC.Api.Models;
using System.Data;


namespace OTC.Api.Services
{
    public class CustodianMasterService : ICustodianMasterService
    {
        public readonly string _connectionString;


        public CustodianMasterService (IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new Exception("Connection string 'DefaultConnection' not found in configuration.");
        }



       
        public async Task<IEnumerable<CustodianMaster>> GetCustodianMasterList(CustodianMasterRequest request)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);

                return await connection.QueryAsync<CustodianMaster>("India1_usp_GetCustodianData", new { Field = (string)null, StartsWith = (string)null, chklocked = (string)null, UserName = request.UserName }, commandType: CommandType.StoredProcedure);

            }
            catch (Exception ex)
            {
                throw new Exception("Failed Fetch Custodian Master List", ex);
            }
        }

        public async Task<CustodianMaster> GetCustodianMasterDetails(StringIdRequest request)
        {


            try
            {
                using var connection = new SqlConnection(_connectionString);

                var parameters = new DynamicParameters();

                return await connection.QueryFirstOrDefaultAsync<CustodianMaster>("India1_usp_CustodianMaster_GetByCode", new { CustodianCode = request.Id }, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed Fetch Custodian Master Details For One Record", ex);
            }

        }


        public async Task<string> UpsertCustodianmaster(CustodianMaster request)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);

                var parameters = new DynamicParameters();
                parameters.Add("@Module", string.IsNullOrEmpty(request.CustodianCode) ? "INSERT" : "UPDATE");
                parameters.Add("@CustodianCode", request.CustodianCode);
                parameters.Add("@CustodianID", request.Id);
                parameters.Add("@CustodianName", request.CustodianName);
                parameters.Add("@MobileNo", request.MobileNumber);
                parameters.Add("@EmailID", request.EmailId);
                parameters.Add("@LocationName", request.LocationName);
                parameters.Add("@FranschiseName", request.FranchiseName);
                parameters.Add("@Zomname", request.ZomName);
                parameters.Add("@RouteKeyId", request.RouteKeyId);
                parameters.Add("@RouteKeyName", request.RouteKeyName);
                parameters.Add("@TouchKeyID", request.TouchKeyId);
                parameters.Add("@IEMI_No", request.IemiNo);
                parameters.Add("@Photo", request.ProfileImage);
                parameters.Add("@AccessFrom", request.AccessFrom);
                parameters.Add("@AccessTo", request.AccessTo);
                parameters.Add("@Custodian_CreatedBy", request.CreatedBy);
                parameters.Add("@Custodian_modifiedBy", request.ModifiedBy);

                await connection.ExecuteAsync("India1_usp_CustodianMaster_Upsert", parameters,commandType: CommandType.StoredProcedure);
                return "Success";
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
