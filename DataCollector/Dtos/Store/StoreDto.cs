using DataCollector.Models;
using System.Text.Json.Serialization;

namespace DataCollector.Dtos.Store
{
    public class StoreDto : AuditableEntity
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = null!;
        [JsonPropertyName("categoryId")]
        public int CategoryId { get; set; }
        [JsonPropertyName("categoryName")]
        public string CategoryName { get; set; } = null!;
        [JsonPropertyName("categoryCode")]
        public string CategoryCode { get; set; } = null!;
        [JsonPropertyName("collectorNote")]
        public string? CollectorNote { get; set; }
        [JsonPropertyName("latitude")]
        public string Latitude { get; set; } = null!;
        [JsonPropertyName("longitude")]
        public string Longitude { get; set; } = null!;
        [JsonPropertyName("isThereANearbyStore")]
        public bool IsThereANearbyStore { get; set; }
        public string CreatorName { get; set; }
        [JsonPropertyName("assets")]
        public List<StorePhotoDto> Assets { get; set; } = new();
    }

    public class StorePhotoDto
    {
        [JsonPropertyName("assetId")]
        public int AssetId { get; set; }
        [JsonPropertyName("assetURL")]
        public string AssetURL { get; set; }
    }

    
}
