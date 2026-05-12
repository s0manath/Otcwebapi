using Dapper;
using Microsoft.Data.SqlClient;
using OTC.Api.Models;
using System.Data;
using static System.Net.WebRequestMethods;

namespace OTC.Api.Services
{
    public class LoginPendingService:ILoginPendingService
    {
        private readonly string _connectionString;


        public LoginPendingService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new Exception("Connection string 'DefaultConnection' not found in configuration.");
        }

        public async Task<LoginPendingModels> GetLoginPendingDeatils(StringIdRequest request)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);

                return await connection.QueryFirstOrDefaultAsync<LoginPendingModels>("India1_usp_GetPendingRequestDetails", new { id = request.Id }, commandType: CommandType.StoredProcedure);

            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public async  Task<IEnumerable<LoginPendingModels>> GetLoginPendingList()
        {
            try
            {
                using var connection =new SqlConnection(_connectionString);

                return await connection.QueryAsync<LoginPendingModels>("usp_GetPendingLoginRequest", new { Field=(string)null, StartsWith=(string)null }, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }
    }
}
