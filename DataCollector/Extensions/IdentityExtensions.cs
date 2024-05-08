using DataCollector.Models.Identity;
using System.Security.Claims;

namespace DataCollector.Extensions
{
    public static class IdentityExtensions
    {
        public static ApplicationUserData GetCurrentUser(this HttpContext httpContext)
        {
            if (httpContext.User == null)
                return null;
            var ss = httpContext.User.Claims;
            var user = new ApplicationUserData();
            user.UserId = httpContext.User.Claims.Single(x => x.Type == "id").Value;
            user.Email = httpContext.User.Claims.Single(x => x.Type == ClaimTypes.Email).Value;
            user.PhoneNumber = httpContext.User.Claims.Single(x => x.Type == "PhoneNumber").Value;
            user.FullName = httpContext.User.Claims.SingleOrDefault(x => x.Type == "FullName")?.Value?? user.Email;
            user.UserName = httpContext.User.Claims.Single(x => x.Type == ClaimTypes.Email).Value;
            user.PhotoURL = httpContext.User.Claims.SingleOrDefault(x => x.Type == "PhotoURL")?.Value ?? "";
            user.Language= httpContext.User.Claims.SingleOrDefault(x => x.Type == "Language")?.Value ?? "en";
            user.Target= httpContext.User.Claims.SingleOrDefault(x => x.Type == "Target")?.Value ?? "0";
            user.UserRole= httpContext.User.Claims.SingleOrDefault(x => x.Type == "UserRole")?.Value ?? "";
            user.TokenVersion = httpContext.User.Claims.SingleOrDefault(x => x.Type == "TokenVersion")?.Value ?? "";
            user.Roles = httpContext.User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToArray();

            return user;
        }
    }
}
