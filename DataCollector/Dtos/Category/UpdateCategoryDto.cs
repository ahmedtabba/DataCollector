using System.Text.Json.Serialization;

namespace DataCollector.Dtos.Category
{
    public class UpdateCategoryDto
    {
        [JsonPropertyName("code")]
        public string Code { get; set; } = null!;
        [JsonPropertyName("name")]
        public string Name { get; set; } = null!;
    }
}
