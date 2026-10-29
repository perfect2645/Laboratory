using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using System.Text.Json;

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
        problem.Detail ??= status >= 500
            ? "An unexpected server error occurred. Please try again later."
            : "The request could not be completed.";

        problem.Extensions.TryAdd("code", status switch
        {
            400 => "INVALID_REQUEST",
            401 => "UNAUTHORIZED",
            403 => "FORBIDDEN",
            404 => "NOT_FOUND",
            405 => "METHOD_NOT_ALLOWED",
            409 => "CONFLICT",
            415 => "UNSUPPORTED_MEDIA_TYPE",
            422 => "UNPROCESSABLE_ENTITY",
            429 => "RATE_LIMITED",
            500 => "INTERNAL_SERVER_ERROR",
            502 => "BAD_GATEWAY",
            503 => "SERVICE_UNAVAILABLE",
            504 => "GATEWAY_TIMEOUT",
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
            options: (JsonSerializerOptions?)null,
            contentType: ContentType,
            cancellationToken: context.RequestAborted);
    }
}
