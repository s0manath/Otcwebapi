using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Data.SqlClient.Diagnostics;
using OTC.Api.Models;
using System.Data;
using System.Reflection.Metadata;

namespace OTC.Api.Services
{
    public class BankMasterService : IBankMasterService
    {
        private readonly string _connectionString;

        public BankMasterService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new Exception("Connection string 'DefaultConnection' not found in configuration.");
        }

        

        public async Task<IEnumerable<Bankmaster>> GetBankMasterList()
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);

                return await connection.QueryAsync<Bankmaster>("India1_usp_GetBankData", new { Field = (string)null, StartsWith = (string)null }, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching bank master list", ex);
            }
        }

        public async  Task<IEnumerable<Bankmaster>> GetBankMasterDetails(StringIdRequest request)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                return await connection.QueryAsync<Bankmaster>("India1_Usp_getbankdetailsbycode", new { bank = request.Id }, commandType: CommandType.StoredProcedure);
            }catch(Exception ex)
            {
                throw new Exception("Error fatching bank Master details", ex);
            }
        }


        public async Task<string> UpsertBankMasterDeatils(Bankmaster bank)
        {
            using var connection = new SqlConnection(_connectionString);

            try
            {
                var parameters = new DynamicParameters(bank);
                parameters.Add("Module", string.IsNullOrEmpty(bank.BankCode) ? "Insert" : "Update");
                await connection.ExecuteAsync("sp_Bankmast",parameters,commandType: CommandType.StoredProcedure);
                return "Success";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }


    }
}
