using Microsoft.AspNetCore.Mvc;
using OTC.Api.Models;

namespace OTC.Api.Services
{
    public interface IKeyInventoryService
    {
        Task<KeyInventory> GetKeyInventoryDeatails( StringIdRequest request);
        Task<IEnumerable<KeyInventory>> GetKeyInventoryList();
        Task<string> UpserttKeyInventory(KeyInventory request);
    }
}
