using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using NetUtils.Aspnet.Errors;

namespace NetUtils.Aspnet.Filters;

public class GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger, IWebHostEnvironment hostEnvironment)
    : IAsyncExceptionFilter
{
    public Task OnExceptionAsync(ExceptionContext context)
    {
        // A started response cannot be replaced. Let the exception middleware handle aborted requests.
        if (context.HttpContext.Response.HasStarted
            || context.Exception is OperationCanceledException && context.HttpContext.RequestAborted.IsCancellationRequested)
        {
            return Task.CompletedTask;
        }

        logger.LogError(
            context.Exception,
            "Unhandled controller exception. TraceId: {TraceId}",
            ApiProblemDetails.GetTraceId(context.HttpContext));

        var problem = ApiProblemDetails.FromException(context.HttpContext, context.Exception, hostEnvironment);
        context.Result = new ObjectResult(problem)
        {
            StatusCode = problem.Status,
            ContentTypes = { ApiProblemDetails.ContentType }
        };
        context.ExceptionHandled = true;

        return Task.CompletedTask;
    }
}

// Retained for source compatibility with existing consumers; the filter now returns ProblemDetails.
public class ErrorResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? Details { get; set; }
}
