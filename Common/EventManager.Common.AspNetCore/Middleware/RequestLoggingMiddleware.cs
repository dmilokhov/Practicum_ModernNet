using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace EventManager.Common.AspNetCore.Middleware;

public class RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        logger.LogInformation("Request path: {Path} ({Method})", context.Request.Path, context.Request.Method);

        await next(context);

        logger.LogInformation("Request finished. Status Code:{StatusCode}", context.Response.StatusCode);
    }
}
