using System;
using System.Collections.Generic;

namespace OTC.Api.Models
{
    public class BulkUploadResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int TotalRecords { get; set; }
        public int ValidRecords { get; set; }
        public int InvalidRecords { get; set; }
        public List<BulkError> Errors { get; set; } = new List<BulkError>();
    }

    public class BulkError
    {
        public int RowNumber { get; set; }
        public string ColumnName { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class BulkUploadRequest
    {
        public string FileName { get; set; }
        public byte[] FileContent { get; set; } // Using base64 or byte array for simple mock implementation
    }
}
