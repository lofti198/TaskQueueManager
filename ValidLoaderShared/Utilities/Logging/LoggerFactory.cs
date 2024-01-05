using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValidLoaderShared.Utilities.Logging
{
    [Flags]
    public enum LogType
    {
        None = 0,
        Console = 1,
        Serilog = 2
    }

    public class NullLogger : LoggerBase
    {
        public override void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            // No action taken for logging
        }
    }


    public static class VL_LoggerFactory
    {
        public static ISimplifiedLogger CreateLogger(LogType logType, string appName, bool includeStackTrace)
        {
            Microsoft.Extensions.Logging.ILogger baseLogger;


            if (logType == LogType.None)
            {
                baseLogger = new NullLogger(); // Default to a minimal logger
            }
            else
            {
                baseLogger = new NullLogger(); // Start with a minimal logger

                if (logType.HasFlag(LogType.Serilog))
                {
                    var logFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "VL_TaskQueueManager", "logs");
                    var logFilePath = Path.Combine(logFolderPath, $"{appName}_logs.txt");

                    // Ensure the logs directory exists
                    Directory.CreateDirectory(logFolderPath);

                    var serilogBaseLogger = new LoggerConfiguration()
                        .MinimumLevel.Debug()
                        .WriteTo.Console()
                        .WriteTo.File(logFilePath, rollingInterval: RollingInterval.Day)
                        .CreateLogger();

                    baseLogger = new SerilogLogger(baseLogger); // Wrap with SerilogLogger
                    Log.Logger = serilogBaseLogger;
                }

                if (logType.HasFlag(LogType.Console))
                {
                    baseLogger = new ConsoleLogger(baseLogger, includeStackTrace); // Wrap with ConsoleLogger
                }
            }

            return baseLogger as ISimplifiedLogger;
        }
    }

}
