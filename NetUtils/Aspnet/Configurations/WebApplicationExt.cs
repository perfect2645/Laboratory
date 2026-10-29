using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NetUtils.Aspnet.Configurations.Swagger;
using NetUtils.Aspnet.Errors;

namespace NetUtils.Aspnet.Configurations
{
    public static class WebApplicationExt
    {
        public static void ConfigApp(this WebApplication app)
        {
            // Handle middleware/result-execution exceptions as JSON in every environment.
            // Controller exceptions are handled by GlobalExceptionFilter using the same contract.
            app.UseExceptionHandler(new ExceptionHandlerOptions
            {
                ExceptionHandler = async context =>
                {
                    var exception = context.Features.Get<IExceptionHandlerFeature>()!.Error;

                    app.Logger.LogError(
                        exception,
                        "Unhandled request exception. TraceId: {TraceId}",
                        ApiProblemDetails.GetTraceId(context));

                    var problem = ApiProblemDetails.FromException(
                        context,
                        exception,
                        app.Environment);

                    await ApiProblemDetails.WriteAsync(context, problem);
                },
                SuppressDiagnosticsCallback = _ => true
            });

            // Fill empty 4xx/5xx responses without replacing explicit response bodies or headers.
            app.UseStatusCodePages(async statusContext =>
            {
                var context = statusContext.HttpContext;
                var factory = context.RequestServices.GetRequiredService<ProblemDetailsFactory>();
                var problem = factory.CreateProblemDetails(context, context.Response.StatusCode);

                await ApiProblemDetails.WriteAsync(context, problem);
            });

            if (app.Environment.IsDevelopment())
            {
                app.UseSwaggerExt();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCorsExt();
            app.UseAuthorization();

            app.MapControllers();
        }
    }
}
