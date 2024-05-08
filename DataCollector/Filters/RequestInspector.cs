using Microsoft.AspNetCore.Mvc.Filters;

namespace DataCollector.Filters
{
    public class RequestInspector : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var request = context.HttpContext.Request;

            // Log or modify the request here...

            await next();
        }
    }

}
