using System.Text.Json.Serialization;

namespace DataCollector.Dtos.Store
{
    public class UpdateStoreDto
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = null!;
        [JsonPropertyName("categoryId")]
        public int CategoryId { get; set; }
        [JsonPropertyName("latitude")]
        public string Latitude { get; set; } = null!;
        [JsonPropertyName("longitude")]
        public string Longitude { get; set; } = null!;
        [JsonPropertyName("newAssets")]
        public List<IFormFile> NewAssets { get; set; } = new();
        [JsonPropertyName("assetsIds")]
        public List<int> AssetsIds { get; set; }= new();

    }


}
