using DataCollector.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using static DataCollector.Utilities.Enums;

namespace DataCollector.Models.Identity
{
    public class RegisterViewModel
    {
        [EmailAddress]
        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }
        [JsonPropertyName("phoneNumber")]
        public string? PhoneNumber { get; set; }
        [JsonPropertyName("target")]
        public int? Target { get; set; }
        [ModelBinder(BinderType = typeof(UserRoleModelBinder))]
        public UserRole UserRole { get; set; }
        public string? Language { get; set; }
    }
}
