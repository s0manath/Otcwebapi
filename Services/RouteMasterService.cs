using Dapper;
using Microsoft.Data.SqlClient;
using OTC.Api.Models;
using System.Data;

namespace OTC.Api.Services
{
    public class RouteMasterService:IRouteMasterService
    {

        private readonly string _connectionString;

        public RouteMasterService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new Exception("Connection string 'DefaultConnection' not found.");
        }

        public async Task<RouteMaster> GetRouteMasterDetails(StringIdRequest request)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);

                return await connection.QueryFirstOrDefaultAsync<RouteMaster>("", new { routeId = request.Id }, commandType: CommandType.StoredProcedure);

            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<RouteMaster>> GetRouteMasterList()
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);

                return await connection.QueryAsync<RouteMaster>("India1_usp_RouteMasterData", commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public async  Task<string> UpsertRoutemaster(RouteMaster request)
        {
            try
            {
                return "ok";
            }
            catch (Exception ex )
            {

                throw new Exception(ex.Message);
            }
        }
    }
}
