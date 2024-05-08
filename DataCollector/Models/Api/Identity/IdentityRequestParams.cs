using System.Text.Json;
using System.Text.Json.Serialization;

namespace DataCollector.Models.Api.Identity
{
    public class IdentityRequestParams : PaginationFilter
    {
        [JsonPropertyName("searchText")]
        public string? SearchText { get; set; }
        [JsonPropertyName("sort")]
        public string? Sort { get; set; }

        public IdentityRequestParams() : base()
        {

        }
        public IdentityRequestParams(int pageNumber, int pageSize, string searchText) : base(pageNumber, pageSize)
        {
            SearchText = searchText;
        }
    }
}
