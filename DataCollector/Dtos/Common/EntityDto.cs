using System.Text.Json.Serialization;

namespace DataCollector.Dtos.Common
{
    public class EntityDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

    }
}
