using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace NetUtils.Aspnet.Configurations
{
    public static class CorsConfig
    {
        public static void AllowCorsExt(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("All", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials(); // 如果前端发送凭据（如cookies）
                });
                options.AddPolicy("AllowReactApp", policy =>
                {
                    policy.WithOrigins("http://localhost:3000")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials(); // 如果前端发送凭据（如cookies）
                });
            });
        }
        public static void UseCorsExt(this IApplicationBuilder app)
        {
            app.UseCors("All");
        }
    }
}
