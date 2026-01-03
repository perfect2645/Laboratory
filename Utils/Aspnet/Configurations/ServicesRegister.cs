using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Utils.Aspnet.Filters;
using Utils.Ioc;

namespace Utils.Aspnet.Configurations
{
    public static class ServicesRegister
    {
        public static void RegisterCommonServices(this WebApplicationBuilder builder)
        {
            //builder.UseAutofac();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddControllers(options =>
            {
                options.Filters.Add<GlobalExceptionFilter>();
            });

            builder.Services.ConfigureRoutes();
        }

        public static void RegisterAssembliesAutofac(this WebApplicationBuilder builder, Assembly[] assemblies)
        {
            builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

            builder.Host.ConfigureContainer<ContainerBuilder>((context, containerBuilder) =>
            {
                containerBuilder.RegisterModule(new AutoRegisterModule(
                    assemblies
                //Assembly.GetExecutingAssembly(),
                //typeof(IShirtRepository).Assembly
                ));
            });
        }
    }
}
