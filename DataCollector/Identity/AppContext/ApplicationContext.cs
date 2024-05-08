using DataCollector.Data;
using DataCollector.Models.Identity;
using System.Security.Claims;

namespace DataCollector.Identity.AppContext
{
    public class ApplicationContext : IApplicationContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ApplicationUserData applicationUserData;

        public AppDbContext Context { get; }

        public ApplicationContext() { }

        public ApplicationContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public ApplicationContext(IHttpContextAccessor httpContextAccessor, AppDbContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            Context = context;
        }
        public ApplicationContext(IHttpContextAccessor httpContextAccessor,
            ApplicationUserData _applicationUserData)
        {
            _httpContextAccessor = httpContextAccessor;
            this.applicationUserData = _applicationUserData;
        }
        public string GetUserId()
        {
            //return "";
            var id = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Name) == null ? _httpContextAccessor.HttpContext.User.FindFirst("id").Value : _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            if (id != null)
                return id;
            return "";
        }

        public ApplicationUserData GetApplicationUserData()
        {
            var user = new ApplicationUserData();
            if (_httpContextAccessor.HttpContext != null && _httpContextAccessor.HttpContext.User != null)
            {
                var prinicpal = _httpContextAccessor.HttpContext.User;

                var x = prinicpal.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).ToArray();
                if (prinicpal != null)
                    user = new ApplicationUserData
                    {
                        Email = prinicpal.Claims.Where(c => c.Type == ClaimTypes.Email).Select(c => c.Value).FirstOrDefault(),
                        UserId = prinicpal.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).FirstOrDefault(),
                        FullName = prinicpal.Claims.Where(c => c.Type == "FullName").Select(c => c.Value).FirstOrDefault(),
                        PhotoURL = prinicpal.Claims.Where(c => c.Type == "PhotoURL").Select(c => c.Value).FirstOrDefault(),
                        UserName = prinicpal.Claims.Where(c => c.Type == ClaimTypes.Name).Select(c => c.Value).FirstOrDefault(),
                        Roles = prinicpal.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToArray(),
                    };
              

            }
            else if (applicationUserData != null)
                user = applicationUserData;

            return user;
        }

     

        public bool IsInRole(string roleName)
        {
            if (_httpContextAccessor.HttpContext.User != null)
            {
                var prinicpal = _httpContextAccessor.HttpContext.User;
                return prinicpal.IsInRole(roleName);
            }
            return false;
        }


        public ApplicationUserData GetUsersByUserId(string userId)
        {
            ApplicationUser user;

            user = Context.Users.Where(c => c.Id == userId).FirstOrDefault();

            return new ApplicationUserData
            {
                UserId = user.Id,
                Email = user.Email,
                FullName = user.UserName,
                UserName = user.UserName,
            };
        }
    }
}
