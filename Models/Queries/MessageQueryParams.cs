namespace Poochatting.Models.Queries
{
    public class MessageQueryParams
    {
        public int PageSize { get; set; }
        public int PageNumber { get; set; }

        public string? OrderBy { get; set; }
        public string? Direction { get; set; }
    }
}
