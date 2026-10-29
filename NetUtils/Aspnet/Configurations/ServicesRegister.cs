using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using NetUtils.Aspnet.Errors;
using NetUtils.Aspnet.Filters;
using System.Reflection;
using System.Text.Json.Serialization;
using Utils.Ioc;

namespace NetUtils.Aspnet.Configurations
{
    public static class ServicesRegister
    {
        public static void RegisterCommonServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddControllers(options =>
            {
                options.Filters.Add<GlobalExceptionFilter>();

            }).AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                options.JsonSerializerOptions.WriteIndented = true;
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

            builder.Services.AddTransient<ProblemDetailsFactory, ApiProblemDetailsFactory>();
            builder.Services.AddProblemDetails(options =>
                options.CustomizeProblemDetails = context =>
                    ApiProblemDetails.ApplyDefaults(context.HttpContext, context.ProblemDetails));

            builder.Services.ConfigureRoutes();
        }

        public static void RegisterAssembliesAutofac(this WebApplicationBuilder builder, Assembly[] assemblies)
        {
            builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

            builder.Host.ConfigureContainer<ContainerBuilder>((_, containerBuilder) =>
            {
                containerBuilder.RegisterModule(new AutoRegisterModule(
                    assemblies
                ));
            });
        }
    }
}
