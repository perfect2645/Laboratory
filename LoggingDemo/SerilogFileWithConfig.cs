using Logging;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace LoggingDemo
{
    internal class SerilogFileWithConfig
    {
        public SerilogFileWithConfig()
        {
            // Configure Serilog
            LogFromConfiguration();
            LogFileException();
        }

        private void LogFromConfiguration()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("serilog.config.json")
                .Build();

            SeriLogger.ReadFromConfiguration(configuration);

            const string method = "LogFromConfiguration";
            Log.Information("{LogFromConfiguration} Information", method);
            Log.Warning("{LogFromConfiguration} Warning", method);
            Log.Error("{LogFromConfiguration} Error", method);
        }

        private void LogFileException()
        {
            try
            {
                throw new Exception("test exception");
            }
            catch (Exception ex)
            {

                const string method = "LogFileException";
                Log.Error(ex, "{LogFileException} Error", method);
            }
        }
    }
}
