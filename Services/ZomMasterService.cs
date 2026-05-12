using Dapper;
using Microsoft.Data.SqlClient;
using OTC.Api.Models;
using System.Data;

namespace OTC.Api.Services
{
    public class ZomMasterService : IZomMasterService
    {

        private readonly string _connectionString;

        public ZomMasterService(IConfiguration configuration)
        {
            _connectionString=configuration.GetConnectionString("DefaultConnection")??
                throw new Exception("Connection string 'DefaultConnection' not found in configuration.");
        }

        public async  Task<IEnumerable<ZomMaster>> GetZomMasterList()
        {
            try
            {
                using var connection =new SqlConnection(_connectionString);

                return await connection.QueryAsync<ZomMaster>("India1_zommasterlist", commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public async Task<ZomMaster> GetZomMasterDetails(StringIdRequest request)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);

                return await connection.QueryFirstOrDefaultAsync<ZomMaster>("India1_zommasterDetails", new {ZomCode=request.Id} ,commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public async Task<string> UpsertZomMaster(ZomMaster request)
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
