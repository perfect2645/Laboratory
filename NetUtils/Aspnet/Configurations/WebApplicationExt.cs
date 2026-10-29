using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetUtils.Aspnet.Configurations.Swagger;
using NetUtils.Aspnet.Errors;

namespace NetUtils.Aspnet.Configurations
{
    public static class WebApplicationExt
    {
        extension(WebApplication app)
        {
            public void ConfigApp()
            {

                // Handle middleware/result-execution exceptions as JSON in every environment.
                // Controller exceptions are handled by GlobalExceptionFilter using the same contract.
                app.UseExceptionHandler(handler =>
                    handler.Run(context =>
                    {
                        var exception = context.Features.Get<IExceptionHandlerFeature>()!.Error;
                        return ApiProblemDetails.WriteAsync(context,
                            ApiProblemDetails.FromException(context, exception, app.Environment));
                    })
                );

                // Fill empty 4xx/5xx responses without replacing explicit response bodies or headers.
                app.UseStatusCodePages(async statusContext =>
                {
                    var context = statusContext.HttpContext;
                    var factory = context.RequestServices.GetRequiredService<ProblemDetailsFactory>();
                    var problem = factory.CreateProblemDetails(context, context.Response.StatusCode);
                    await ApiProblemDetails.WriteAsync(context, problem);
                });

                app.UseCorsExt();
                // Configure the HTTP request pipeline.
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
                app.UseAuthorization();
                app.MapControllers();
            }
        }
    }
}
