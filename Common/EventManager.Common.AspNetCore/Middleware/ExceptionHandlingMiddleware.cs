using EventManager.Common.Core.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace EventManager.Common.AspNetCore.Middleware
{
    public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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

            var problemDetails = ex switch
            {
                ApiException ae => new ProblemDetails
                { 
                    Detail = ae.Message,
                    Status = ae.StatusCode
                },
                ValidationException fve => new ProblemDetails
                { 
                    Detail = fve.Message,
                    Status = StatusCodes.Status400BadRequest
                },
                _ => new ProblemDetails
                {
                    Detail = "Internal server error",
                    Status = StatusCodes.Status500InternalServerError
                }
            };


            var apiErrorResponse = new ApiErrorResult
            {
                Message = "Exception occurred. See Error Details",
                ErrorDetails = problemDetails
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)apiErrorResponse.ErrorDetails.Status!;

            await context.Response.WriteAsJsonAsync(apiErrorResponse);
        }
    }
}
