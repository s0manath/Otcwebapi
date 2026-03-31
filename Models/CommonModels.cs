namespace OTC.Api.Models
{
    public class IdRequest
    {
        public int Id { get; set; }
    }

    public class StringIdRequest
    {
        public string Id { get; set; } = string.Empty;
    }

    public class LongIdRequest
    {
        public long Id { get; set; }
    }

    public class SearchRequest
    {
        public string Query { get; set; } = string.Empty;
    }
}
