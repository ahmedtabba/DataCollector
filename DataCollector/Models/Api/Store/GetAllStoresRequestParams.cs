using System.Text.Json.Serialization;

namespace DataCollector.Models.Api.Store
{
    public class GetAllStoresRequestParams : PaginationFilter
    {
        [JsonPropertyName("searchText")]
        public string? SearchText { get; set; }
        [JsonPropertyName("sort")]
        public string? Sort { get; set; }
        [JsonPropertyName("categoryId")]
        public int? CategoryId { get; set; }
        [JsonPropertyName("createdByCurrentUser")]
        public bool? CreatedByCurrentUser{ get; set; }

        public GetAllStoresRequestParams() : base()
        {

        }
        public GetAllStoresRequestParams(int pageNumber, int pageSize, string? searchText, bool? createdByCurrentUser) : base(pageNumber, pageSize)
        {
            SearchText = searchText;
            CreatedByCurrentUser = createdByCurrentUser;
        }
    }
}
