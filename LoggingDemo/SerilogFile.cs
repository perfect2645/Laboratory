using Logging;
using Serilog;

namespace LoggingDemo
{
    internal class SerilogFile
    {
        public SerilogFile()
        {
            // Configure Serilog
            //SeriLogger.WriteToFile();
            //LogFile();
            LogEnrichFile();
            LogFileException();
        }

        private void LogFile()
        {
            const string method = "LogFile";
            Log.Information("{LogFile}Information", method);
            Log.Warning("{LogFile}Warning", method);
            Log.Error("{LogFile}Error", method);
        }

        private void LogEnrichFile()
        {
            SeriLogger.WriteToFile(logPath: "logs/demo-.log",
                configLog: loggerConfiguration =>
            {
                loggerConfiguration.Enrich.WithProperty("Application", "LoggingDemo");
            });
            const string method = "LogEnrichFile";
            Log.Information("{LogEnrichFile} Information", method);
            Log.Warning("{LogEnrichFile} Warning", method);
            Log.Error("{LogEnrichFile} Error", method);
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
