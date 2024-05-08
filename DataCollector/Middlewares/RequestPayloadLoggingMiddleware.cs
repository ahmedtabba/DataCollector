using System.Text;

namespace DataCollector.Middlewares
{
    public class RequestPayloadLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestPayloadLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if ((context.Request.Method == "POST"|| context.Request.Method == "PUT") && context.Request.ContentLength.HasValue && context.Request.ContentLength > 0)
            {
                context.Request.EnableBuffering(); // Enable buffering to read the request body without affecting the request processing

                using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true);
                var requestBody = await reader.ReadToEndAsync();

                context.Request.Body.Position = 0; // Reset the body stream position for further request processing

                //Log.Information("Method: {Method}, Path: {Path}, Query: {Query}, Payload: {Payload}, StatusCode: {StatusCode}",
                //    context.Request.Method,
                //    context.Request.Path,
                //    context.Request.QueryString,
                //    requestBody,
                //    context.Response.StatusCode);
            }

            await _next(context);
        }
    }
}
