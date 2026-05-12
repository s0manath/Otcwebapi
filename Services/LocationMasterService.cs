using Dapper;
using Microsoft.Data.SqlClient;
using OTC.Api.Models;
using System.Data;

namespace OTC.Api.Services
{
    public class LocationMasterService : ILocationMasterService
    {
        public readonly string _connectionString;

        public LocationMasterService(IConfiguration configuration )
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
           ?? throw new Exception("Connection string 'DefaultConnection' not found.");
        }

        public async Task<LocationMaster> GetLocationMasterDetails(StringIdRequest request)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);

                //var parameters = new DynamicParameters();

                return await connection.QueryFirstOrDefaultAsync<LocationMaster>(
                 "India1_LocationMaster_Details",
                 new { LocationCode = request.Id },
                 commandType: CommandType.StoredProcedure
             );
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }
        }

        public async Task<IEnumerable<LocationMaster>> GetLocationMasterList()
        {
           

            try
            {
                using var connection = new SqlConnection(_connectionString);

                var parameters = new DynamicParameters();

                return await connection.QueryAsync<LocationMaster>("India1_LocationMaster_get", parameters, commandType: CommandType.StoredProcedure);


            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        //public Task GetLocationMasterUpsert()
        //{
        //    try
        //    {

        //    }
        //    catch (Exception ex)
        //    {

        //        throw new Exception(ex.Message);
        //    }
        //}

        
    }
}
