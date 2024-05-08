using System.Text.Json.Serialization;
using static DataCollector.Utilities.Enums;

namespace DataCollector.Models.Identity
{
    public class ApplicationUserResponsVM
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string? FullName { get; set; }
        public bool HasPhoto { get; set; }
        public string? PhotoURL { get; set; }
        public string? PhoneNumber { get; set; }
        public string? UserName { get; set; }
        public int? Target { get; set; }
        public bool IsActive { get; set; } = true;
        public UserRole UserRole { get; set; }
        public bool LockoutEnabled { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public string? Language { get; set; }

    }
}
