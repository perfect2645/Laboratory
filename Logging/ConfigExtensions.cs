using Serilog;

namespace Logging
{
    public static class ConfigExtensions
    {
        extension(LoggerConfiguration loggerConfiguration)
        {
            public LoggerConfiguration ConfigWriteToFile(string? logPath = null,
                RollingInterval rollingInterval = RollingInterval.Day)
            {
                logPath = logPath ?? "logs/app-.log";

                loggerConfiguration.MinimumLevel.Information()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File(logPath, rollingInterval: rollingInterval);

                return loggerConfiguration;
            }
        }
    }
}
