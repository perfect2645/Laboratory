using Microsoft.Extensions.Hosting;
using Serilog;

namespace Logging
{
    public static class HostingLogger
    {

        extension(IHostBuilder hostBuilder)
        {
            public IHostBuilder AddSerilogger()
            {
                hostBuilder.UseSerilog((host, loggerConfiguration) =>
                {
                    loggerConfiguration.ReadFrom.Configuration(host.Configuration)
                        .Enrich.FromLogContext()
                        .WriteTo.Console();
                });
                return hostBuilder;
            }
        }
    }
}
