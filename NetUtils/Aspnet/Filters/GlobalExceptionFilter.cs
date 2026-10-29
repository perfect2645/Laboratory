using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using NetUtils.Aspnet.Errors;

namespace NetUtils.Aspnet.Filters
{
    public class GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger,
        IWebHostEnvironment hostEnvironment) : IAsyncExceptionFilter
    {
        public Task OnExceptionAsync(ExceptionContext context)
        {
            var httpContext = context.HttpContext;
            var exception = context.Exception;
            // A started response cannot be replaced. Let the exception middleware handle aborted requests.
            if (httpContext.Response.HasStarted
                || (httpContext.RequestAborted.IsCancellationRequested
                    && exception is OperationCanceledException or IOException))
            {
                return Task.CompletedTask;
            }

            var traceId = ApiProblemDetails.GetTraceId(httpContext);

            logger.LogError(
                exception,
                "Unhandled controller exception. TraceId: {TraceId}",
                traceId);

            var problemDetails = ApiProblemDetails.FromException(
                httpContext,
                exception,
                hostEnvironment);

            context.Result = new ObjectResult(problemDetails);
            context.ExceptionHandled = true;

            return Task.CompletedTask;
        }
    }
}
