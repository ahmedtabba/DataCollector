using System.Text.Json;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using static DataCollector.Utilities.Enums;

namespace DataCollector.Models.Identity
{
    public class AddUserViewModel
    {
        [EmailAddress]
        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }

        [JsonPropertyName("phoneNumber")]
        public string? PhoneNumber { get; set; }
        [JsonPropertyName("fullName")]
        public string? FullName { get; set; }

        [JsonPropertyName("photo")]
        public IFormFile? Photo { get; set; }
        public bool? HasPhoto { get; set; }
        public string? PhotoURL { get; set; }

        [JsonPropertyName("target")]
        public int? Target { get; set; }
        [JsonPropertyName("isActive")]
        public bool IsActive { get; set; }
        [JsonPropertyName("userRole")]
        public UserRole UserRole { get; set; }
        public string? Language { get; set; }

    }
}
