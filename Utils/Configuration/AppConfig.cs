using Microsoft.Extensions.Configuration;

namespace Utils.Configuration
{
    public static class AppConfig
    {
        public static IConfiguration? Configuration { get; private set; }

        public static void Init()
        {
            var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
            
            #if DEBUG
                environment ??= "development";
            #else
                environment ??= "production";
            #endif
            
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment ?? "production"}.json",
                    optional: true,
                    reloadOnChange:true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }
    }
}
