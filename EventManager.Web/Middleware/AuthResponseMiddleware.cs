using EventManager.Web.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace EventManager.Web.Middleware
{
    public class AuthResponseMiddleware(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            await next(context);

            if (context.Response is { 
                HasStarted: false, StatusCode: StatusCodes.Status401Unauthorized or StatusCodes.Status403Forbidden })
            {
                await HandleAuthorizationStatusCodeAsync(context);
            }
        }

        private static async Task HandleAuthorizationStatusCodeAsync(HttpContext context)
        {
            var statusCode = context.Response.StatusCode;

            var message = statusCode == StatusCodes.Status401Unauthorized
                ? "You are unauthorized. Please provide valid token"
                : "Access to the operation is denied";

            var apiErrorResponse = new ApiErrorResult
            {
                Created = DateTime.UtcNow,
                Message = "Authorization failed. See Error Details",
                ErrorDetails = new ProblemDetails
                {
                    Detail = message,
                    Status = statusCode,
                    Title = statusCode == StatusCodes.Status401Unauthorized ? "Unauthorized" : "Forbidden",
                    Instance = context.Request.Path
                }
            };

            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(apiErrorResponse);
        }
    }
}
