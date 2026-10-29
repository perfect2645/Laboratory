using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace NetUtils.Aspnet.Errors;

public sealed class ApiProblemDetailsFactory : ProblemDetailsFactory
{
    public override ProblemDetails CreateProblemDetails(
        HttpContext httpContext,
        int? statusCode = null,
        string? title = null,
        string? type = null,
        string? detail = null,
        string? instance = null)
    {
        var problem = new ProblemDetails
        {
            Status = statusCode
                     ?? StatusCodes.Status500InternalServerError,
            Title = title,
            Type = type,
            Detail = detail,
            Instance = instance
        };

        ApiProblemDetails.ApplyDefaults(httpContext, problem);

        return problem;
    }

    public override ValidationProblemDetails CreateValidationProblemDetails(
        HttpContext httpContext,
        ModelStateDictionary modelStateDictionary,
        int? statusCode = null,
        string? title = null,
        string? type = null,
        string? detail = null,
        string? instance = null)
    {
        ArgumentNullException.ThrowIfNull(modelStateDictionary);

        var problem = new ValidationProblemDetails(modelStateDictionary)
        {
            Status = statusCode
                     ?? StatusCodes.Status400BadRequest,
            Title = title
                    ?? "One or more validation errors occurred.",
            Type = type,
            Detail = detail
                     ?? "Check the request fields and try again.",
            Instance = instance,
            Extensions =
            {
                ["code"] = "VALIDATION_ERROR"
            }
        };

        ApiProblemDetails.ApplyDefaults(httpContext, problem);

        return problem;
    }
}
