using Microsoft.AspNetCore.Identity;

namespace DataCollector.Identity
{
    public class ApplicationUser : IdentityUser
    {
        // Add customisations here later
        public bool HasPhoto { get; set; }
        public string? FullName { get; set; }
        public string? PhotoURL { get; set; }
        public string? Token { get; set; }
        public int? Target { get; set; }
        public bool IsActive { get; set; }
        public int UserRole { get; set; }
        public string? Language { get; set; }
        public string? TokenVersion { get; set; }
    }
}
