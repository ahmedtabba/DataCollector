using System.Text.Json;
using System.Text.Json.Serialization;
using static DataCollector.Utilities.Enums;

namespace DataCollector.Models.Identity
{
    public class EditUserVM
    {
        [JsonPropertyName("email")]
        public string Email { get; set; }
        [JsonPropertyName("fullName")]
        public string FullName { get; set; }

        [JsonPropertyName("phoneNumber")]
        public string PhoneNumber { get; set; }

        [JsonPropertyName("photo")]
        public IFormFile Photo { get; set; }
        public bool HasPhoto { get; set; }
        public string PhotoURL { get; set; }
        public int? Target { get; set; }
        public bool IsActive { get; set; }
        [JsonPropertyName("userRole")]
        public UserRole UserRole { get; set; }
        public string? Language { get; set; }
    }
}
