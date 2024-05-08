using System.Text.Json.Serialization;

namespace DataCollector.Models.Api.Store
{
    public class ExportStoresRequestParams 
    {
        
        [JsonPropertyName("categoryId")]
        public int? CategoryId { get; set; }
        [JsonPropertyName("creatorId")]
        public string? CreatorId { get; set; }
        [JsonPropertyName("from")]
        public DateTime? From { get; set; }
        [JsonPropertyName("to")]
        public DateTime? To { get; set; }
       
    }
}
