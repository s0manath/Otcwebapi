using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using OTC.Api.Models;
using System.Data;
using System.Runtime.InteropServices;

namespace OTC.Api.Services
{
    public class KeyInventoryService:IKeyInventoryService
    {
        private readonly string _connectionString;

        public KeyInventoryService(IConfiguration configuration)
        {
            _connectionString=configuration.GetConnectionString("DefaultConnection") ?? throw new Exception("Connection string 'DefaultConnection' not found.");
        }

        public async Task<KeyInventory> GetKeyInventoryDeatails(StringIdRequest request)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);

                return await connection.QueryFirstOrDefaultAsync<KeyInventory>("India1_getkeyinventoryDetails", new {Podno=request.Id}, commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<KeyInventory>> GetKeyInventoryList()
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);

                return await connection .QueryAsync<KeyInventory>("India1_getkeyinventorylist", commandType: CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public async Task<string> UpserttKeyInventory(KeyInventory request)
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
