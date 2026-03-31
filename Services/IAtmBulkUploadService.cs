using System.Threading.Tasks;
using OTC.Api.Models;

namespace OTC.Api.Services
{
    public interface IAtmBulkUploadService
    {
        Task<BulkUploadResponse> ProcessAtmBulkUploadAsync(BulkUploadRequest request);
        Task<BulkUploadResponse> ProcessRouteKeyUpdateAsync(BulkUploadRequest request);
    }
}
