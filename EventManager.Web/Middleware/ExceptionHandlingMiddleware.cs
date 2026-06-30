using EventManager.Application.Interfaces;
using EventManager.Web.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace EventManager.Web.Middleware
{
    public class ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IExceptionMapper exceptionMapper)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Exception occurred! Method = {Method}, Path = {Path}, RequestId = {RequestId}",
                    context.Request.Method,
                    context.Request.Path,
                    context.Request.Headers["x-request-id"]);

                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            if (context.Response.HasStarted)
            {
                return;
            }

            var mapping = exceptionMapper.Map(ex);

            var apiErrorResponse = new ApiErrorResult
            {
                Message = "Exception occurred. See Error Details",
                ErrorDetails = mapping is not null
                    ? new ProblemDetails
                    {
                        Detail = mapping.Detail,
                        Status = mapping.StatusCode,
                    }
                    : new ProblemDetails
                    {
                        Detail = "Internal server error",
                        Status = StatusCodes.Status500InternalServerError,
                    }
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)apiErrorResponse.ErrorDetails.Status!;

            await context.Response.WriteAsJsonAsync(apiErrorResponse);
        }
    }
}
