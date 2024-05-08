namespace DataCollector.Models.Identity
{
    public class ApplicationUserData
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string PhotoURL { get; set; }
        public string Target { get; set; }
        public string? TokenVersion { get; set; }
        public string? Language { get; set; }
        public string UserRole { get; set; }
        public string[] Roles { get; set; }

    }
}
