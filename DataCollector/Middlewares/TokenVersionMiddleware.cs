using DataCollector.Data;
using DataCollector.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using DataCollector.Extensions;

namespace DataCollector.Middlewares
{
    public class TokenVersionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TokenVersionMiddleware(RequestDelegate next, IServiceScopeFactory serviceScopeFactory, IHttpContextAccessor httpContextAccessor)
        {
            _next = next;
            _serviceScopeFactory = serviceScopeFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var endpoint = context.GetEndpoint();
            if (endpoint?.Metadata?.GetMetadata<AuthorizeAttribute>() is not object)
            {
                // If the endpoint allows anonymous access, bypass the middleware logic
                await _next(context);
                return;
            }

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var identityService = scope.ServiceProvider.GetRequiredService<IIdentityService>();
                // Use the identityService here

                var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("id");
                    //.User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return;
                }

                var userId = userIdClaim.Value;
                var tokenVersionClaim = context.User.FindFirst("TokenVersion"); // Assuming "TokenVersion" is the claim type
                if (string.IsNullOrWhiteSpace(tokenVersionClaim?.Value))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return;
                }

                var tokenVersion = tokenVersionClaim.Value;
                var user = identityService.GetUserBtId(userId);
                if (user == null || user.TokenVersion != tokenVersion)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return;
                }

                await _next(context);
            }
        }
    }

}
