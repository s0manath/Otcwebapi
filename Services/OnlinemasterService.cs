using Dapper;
using Microsoft.Data.SqlClient;
using OTC.Api.Models;
using System.Data;

namespace OTC.Api.Services
{
    public class OnlinemasterService:IOnlineMasterService
    {

        private readonly string _connectionString;

        public OnlinemasterService(IConfiguration configuration)

        {
            _connectionString=configuration.GetConnectionString("DefaultConnection")
                ?? throw new Exception("Connection string 'DefaultConnection' not found.");
        }

        public async  Task<OnlineMaster> GetOnlineMasterDetails(StringIdRequest request)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);

                return await connection.QueryFirstOrDefaultAsync<OnlineMaster>("India1_onlinemasterdetails", new { Id = request.Id }, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<OnlineMaster>> GetZomMasterList()
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);

                return await connection.QueryAsync<OnlineMaster>("India1_onlinemasterlist", commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public async  Task<string> UpsertonlineMaster(OnlineMaster request)
        {
            try
            {
                return "ok";
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }
    }
}
