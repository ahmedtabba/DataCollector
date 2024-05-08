using System.Text.Json;
using System.Text.Json.Serialization;

namespace DataCollector.Models.Api
{
    public class PaginationFilter
    {
        [JsonPropertyName("pageNumber")]
        public int PageNumber { get; set; }

        [JsonPropertyName("pageSize")]
        public int PageSize { get; set; }
        public PaginationFilter()
        {
            this.PageNumber = 1;
            this.PageSize = 10;
        }
        public PaginationFilter(int pageNumber, int pageSize)
        {
            this.PageNumber = pageNumber < 1 ? 1 : pageNumber;
            this.PageSize = pageSize == 0 ? 10 : pageSize;
        }
    }
}
