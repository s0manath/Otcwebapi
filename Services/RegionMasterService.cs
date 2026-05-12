using Dapper;
using Microsoft.Data.SqlClient;
using OTC.Api.Models;
using System.Data;

namespace OTC.Api.Services
{
    public class RegionMasterService: IRegionMasterService
    {
        private readonly string _connectionString;

        public RegionMasterService(IConfiguration configuration)
        {
            _connectionString=configuration.GetConnectionString("DefaultConnection") ?? throw new Exception("Connection string 'DefaultConnection' not found.");
        }

        public async Task<RegionMaster> GetRegionMasterDetails(StringIdRequest request)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);

                return await connection.QueryFirstOrDefaultAsync<RegionMaster>("India1_getregionmasterdetails", new {Rocode=request.Id}, commandType: CommandType.StoredProcedure);

            }
            catch (Exception ex)
            {

                throw new Exception("failed to fatch Regionmaster Details", ex);
            }
        }

        public async Task<IEnumerable<RegionMaster>> GetRegionMasterList()
        {
            try
            {
                using var connection=new SqlConnection(_connectionString);

                return await connection.QueryAsync<RegionMaster>("India1_getregionmasterlist", commandType: CommandType.StoredProcedure);
                
            }
            catch (Exception ex)
            {

                throw new Exception("Failed to fatch the  RegionMaster List", ex);
            }
        }

        public async  Task<string> UpsertRegionMaster(RegionMaster request)
        {
            try
            {
                return "ok";
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
