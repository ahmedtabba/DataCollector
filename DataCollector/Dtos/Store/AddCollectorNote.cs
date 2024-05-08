using System.Text.Json.Serialization;

namespace DataCollector.Dtos.Store
{
    public class AddCollectorNote
    {
        [JsonPropertyName("note")]
        public string Note { get; set; }
    }
}
