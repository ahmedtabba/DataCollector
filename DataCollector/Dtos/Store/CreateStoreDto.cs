using DataCollector.Helpers;
using DataCollector.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace DataCollector.Dtos.Store
{
    public class CreateStoreDto
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = null!;
        [JsonPropertyName("categoryId")]
        public int CategoryId { get; set; }
        [JsonPropertyName("latitude")]
        public string Latitude { get; set; } = null!;
        [JsonPropertyName("longitude")]
        public string Longitude { get; set; } = null!;
        [JsonPropertyName("assets")]
        public List<IFormFile> Assets { get; set; } = new();
    }


}
