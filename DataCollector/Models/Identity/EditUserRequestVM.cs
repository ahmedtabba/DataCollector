using System.Text.Json;
using System.Text.Json.Serialization;
using static DataCollector.Utilities.Enums;

namespace DataCollector.Models.Identity
{
    public class EditUserRequestVM
    {
        [JsonPropertyName("email")]
        public string Email { get; set; }
        [JsonPropertyName("fullName")]
        public string? FullName { get; set; }

        [JsonPropertyName("phoneNumber")]
        public string? PhoneNumber { get; set; }

        [JsonPropertyName("photo")]
        public IFormFile? Photo { get; set; }
        [JsonPropertyName("target")]
        public int? Target { get; set; }
        [JsonPropertyName("isActive")]
        public bool IsActive { get; set; } = true;
        [JsonPropertyName("userRole")]
        public UserRole UserRole { get; set; }
        [JsonPropertyName("language")]
        public string? Language { get; set; }

    }
}
