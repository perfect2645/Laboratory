using System.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace NetUtils.Aspnet.Errors;

public static class ApiProblemDetails
{
    public const string ContentType = "application/problem+json";

    public static string GetTraceId(HttpContext context)
        => Activity.Current?.Id ?? context.TraceIdentifier;

    public static void ApplyDefaults(HttpContext context, ProblemDetails problem)
    {
        var status = problem.Status ?? StatusCodes.Status500InternalServerError;
        problem.Status = status;
        problem.Type ??= "about:blank";
        problem.Title ??= ReasonPhrases.GetReasonPhrase(status);
        problem.Instance ??= context.Request.Path.Value;

        problem.Detail ??= status >= StatusCodes.Status500InternalServerError
            ? "An unexpected server error occurred. Please try again later."
            : "The request could not be completed.";

        problem.Extensions.TryAdd("code", status switch
        {
            StatusCodes.Status400BadRequest => "INVALID_REQUEST",
            StatusCodes.Status401Unauthorized => "UNAUTHORIZED",
            StatusCodes.Status403Forbidden => "FORBIDDEN",
            StatusCodes.Status404NotFound => "NOT_FOUND",
            StatusCodes.Status405MethodNotAllowed => "METHOD_NOT_ALLOWED",
            StatusCodes.Status409Conflict => "CONFLICT",
            StatusCodes.Status415UnsupportedMediaType => "UNSUPPORTED_MEDIA_TYPE",
            StatusCodes.Status422UnprocessableEntity => "UNPROCESSABLE_ENTITY",
            StatusCodes.Status429TooManyRequests => "RATE_LIMITED",
            StatusCodes.Status500InternalServerError => "INTERNAL_SERVER_ERROR",
            StatusCodes.Status502BadGateway => "BAD_GATEWAY",
            StatusCodes.Status503ServiceUnavailable => "SERVICE_UNAVAILABLE",
            StatusCodes.Status504GatewayTimeout => "GATEWAY_TIMEOUT",
            _ => "HTTP_ERROR"
        });

        problem.Extensions["traceId"] = GetTraceId(context);
    }

    public static ProblemDetails FromException(HttpContext context, Exception exception, IWebHostEnvironment environment)
    {
        var factory = context.RequestServices.GetRequiredService<ProblemDetailsFactory>();
        var problem = factory.CreateProblemDetails(context, StatusCodes.Status500InternalServerError);

        if (environment.IsDevelopment())
        {
            problem.Detail = exception.GetBaseException().Message;
            problem.Extensions["exception"] = exception.ToString();
        }

        return problem;
    }

    public static Task WriteAsync(HttpContext context, ProblemDetails problem)
    {
        ApplyDefaults(context, problem);
        context.Response.StatusCode = problem.Status!.Value;

        return context.Response.WriteAsJsonAsync(
            problem,
            options: null,
            contentType: ContentType,
            cancellationToken: context.RequestAborted);
    }
}
