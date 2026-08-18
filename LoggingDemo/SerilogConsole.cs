using Logging;
using Serilog;
using Serilog.Exceptions;
using Serilog.Sinks.SystemConsole.Themes;

namespace LoggingDemo
{
    internal class SerilogConsole
    {
        public SerilogConsole()
        {
            // Configure Serilog
            SeriLogger.WriteToConsole();
            LogConsole();
            LogEnrichConsole();
            LogConsoleException();
        }

        private void LogConsole()
        {
            const string method = "LogConsole";
            Log.Information("{LogConsole}Information", method);
            Log.Warning("{LogConsole}Warning", method);
            Log.Error("{LogConsole}Error", method);
        }

        private void LogEnrichConsole()
        {
            SeriLogger.WriteToConsole(loggerConfiguration =>
            {
                loggerConfiguration.Enrich.WithProperty("Application", "LoggingDemo");
            });
            const string method = "LogEnrichConsole";
            Log.Information("{LogEnrichConsole} Information", method);
            Log.Warning("{LogEnrichConsole} Warning", method);
            Log.Error("{LogEnrichConsole} Error", method);
        }

        private void LogConsoleException()
        {
            try
            {
                throw new Exception("test exception");
            }
            catch (Exception ex)
            {

                const string method = "LogConsoleException";
                Log.Error(ex, "{LogConsoleException} Error", method);
            }
        }
    }
}
