using System.Text.Json;
using System.Text.Json.Serialization;

namespace DataCollector.Models.Api.Category
{
    public class GetAllCategoriesRequestParams : PaginationFilter
    {
        [JsonPropertyName("searchText")]
        public string? SearchText { get; set; }
        [JsonPropertyName("sort")]
        public string? Sort { get; set; }
        [JsonPropertyName("prevId")]
        public int? PrevId { get; set; }

        public GetAllCategoriesRequestParams() : base()
        {

        }
        public GetAllCategoriesRequestParams(int pageNumber, int pageSize, string? searchText) : base(pageNumber, pageSize)
        {
            SearchText = searchText;
        }
    }

    
}
