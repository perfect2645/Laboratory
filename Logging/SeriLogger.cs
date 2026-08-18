using Microsoft.Extensions.Configuration;
using Serilog;

namespace Logging
{
    public static class SeriLogger
    {
        public static ILogger Logger = Log.Logger;

        public static void CreateBootstrapLogger(string? logPath = null, RollingInterval rollingInterval = RollingInterval.Day)
        {
            logPath = logPath ?? "logs/bootstrap-.log";

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File(logPath, rollingInterval: rollingInterval)
                .CreateBootstrapLogger();
        }

        public static void WriteToFile(string? logPath = null,
            RollingInterval rollingInterval = RollingInterval.Day,
            Action<LoggerConfiguration>? configLog = null)
        {
            logPath = logPath ?? "logs/app-.log";

            var loggerConfiguration = new LoggerConfiguration()
                .MinimumLevel.Information()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File(logPath, rollingInterval: rollingInterval);

            configLog?.Invoke(loggerConfiguration);

            Log.Logger = loggerConfiguration.CreateLogger();
        }

        public static void WriteToConsole(Action<LoggerConfiguration>? configLog = null)
        {
            var loggerConfiguration = new LoggerConfiguration()
                .MinimumLevel.Information()
                .Enrich.FromLogContext()
                .Enrich.WithThreadId()
                .WriteTo.Console(outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {Level:u3}] {Application} [{ThreadId}] : {Message:lj}{NewLine}{Exception}");

            configLog?.Invoke(loggerConfiguration);

            Log.Logger = loggerConfiguration.CreateLogger();
        }

        public static void WriteToDebug(Action<LoggerConfiguration>? configLog = null)
        {
            var loggerConfiguration = new LoggerConfiguration()
                .MinimumLevel.Information()
                .Enrich.FromLogContext()
                .Enrich.WithThreadId()
                .WriteTo.Debug(outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {Level:u3}] {Application} [{ThreadId}] : {Message:lj}{NewLine}{Exception} {Properties:j}");

            configLog?.Invoke(loggerConfiguration);

            Log.Logger = loggerConfiguration.CreateLogger();
        }

        public static void ReadFromConfiguration(IConfiguration configuration, Action<LoggerConfiguration>? configLog = null)
        {
            var loggerConfiguration = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration);

            configLog?.Invoke(loggerConfiguration);

            Log.Logger = loggerConfiguration.CreateLogger();
        }
    }
}
