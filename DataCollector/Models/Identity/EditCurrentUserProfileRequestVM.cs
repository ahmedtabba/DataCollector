using System.Text.Json.Serialization;

namespace DataCollector.Models.Identity
{
    public class EditCurrentUserProfileRequestVM
    {
        [JsonPropertyName("email")]
        public string Email { get; set; }
        [JsonPropertyName("fullName")]
        public string? FullName { get; set; }

        [JsonPropertyName("phoneNumber")]
        public string? PhoneNumber { get; set; }

        [JsonPropertyName("photo")]
        public IFormFile? Photo { get; set; }
        //[JsonPropertyName("target")]
        //public int? Target { get; set; }
        [JsonPropertyName("language")]
        public string? Language { get; set; }

    }
}
