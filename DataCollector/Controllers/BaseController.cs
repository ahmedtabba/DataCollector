using DataCollector.Extensions;
using DataCollector.Models.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DataCollector.Controllers
{
    public class BaseController : ControllerBase
    {
        public BaseController()
        {
        }
        public ApplicationUserData GetCurrentUser
        {
            get
            {
                return HttpContext.GetCurrentUser();
            }
        }

        protected string GetErrorMessage(Exception ex)
        {
            try
            {
                if (ex.InnerException != null)
                    return GetErrorMessage(ex.InnerException);
                else return ex.Message;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
