using Microsoft.AspNetCore.Identity;

namespace DataCollector.Identity
{
    public class ApplicationRole : IdentityRole
    {
        public ApplicationRole() : base()
        {
        }

        public ApplicationRole(string roleName) : base(roleName)
        {
        }

        // Other properties and methods...
    }

}
