using Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using NetUtils.Aspnet.Configurations.Swagger;
using Serilog;

namespace NetUtils.Aspnet.Configurations
{
    public static class WebApplicationExt
    {
        extension(WebApplication app)
        {
            public void ConfigApp()
            {
                app.UseCorsExt();
                // Configure the HTTP request pipeline.
                if (app.Environment.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                    app.MapOpenApi();
                    app.UseSwaggerExt();
                }
                else
                {
                    // handle exceptions internally in production
                    app.UseExceptionHandler("/error");
                    app.UseHsts();
                }

                app.UseHttpsRedirection();

                app.UseAuthorization();
                app.UseRouting();
                app.MapControllers();

                app.UseErrorEndpoint();
            }

            private void UseErrorEndpoint()
            {
                app.Map("/error", async context =>
                {
                    var exception = context.Features.Get<IExceptionHandlerPathFeature>()?.Error;

                    Log.Error(exception, "Unhandled exception at {context.Request.Path}", context.Request.Path);

                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "application/json";

                    await context.Response.WriteAsync("Server error occurred.");
                });
            }
        }
    }
}
