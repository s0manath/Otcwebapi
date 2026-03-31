using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OTC.Api.Models;

namespace OTC.Api.Services
{
    public class AtmBulkUploadService : IAtmBulkUploadService
    {
        public Task<BulkUploadResponse> ProcessAtmBulkUploadAsync(BulkUploadRequest request)
        {
            // Simulated processing logic
            return Task.FromResult(new BulkUploadResponse
            {
                Success = true,
                Message = "Bulk ATM processing complete (Simulation Mode)",
                TotalRecords = 150,
                ValidRecords = 142,
                InvalidRecords = 8,
                Errors = new List<BulkError>
                {
                    new BulkError { RowNumber = 5, ColumnName = "ATM ID", ErrorMessage = "Duplicate entry in node hierarchy" },
                    new BulkError { RowNumber = 12, ColumnName = "Install Date", ErrorMessage = "Temporal format mismatch" },
                    new BulkError { RowNumber = 24, ColumnName = "ZOM", ErrorMessage = "Target Zone not found" }
                }
            });
        }

        public Task<BulkUploadResponse> ProcessRouteKeyUpdateAsync(BulkUploadRequest request)
        {
            return Task.FromResult(new BulkUploadResponse
            {
                Success = true,
                Message = "Route Key propagation complete (Simulation Mode)",
                TotalRecords = 50,
                ValidRecords = 50,
                InvalidRecords = 0
            });
        }
    }
}
