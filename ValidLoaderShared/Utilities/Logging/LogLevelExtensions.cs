using Microsoft.Extensions.Logging;
using Serilog.Events;

public static class LogLevelExtensions
{
    public static LogEventLevel ToSerilogEventLevel(this LogLevel logLevel)
    {
        return logLevel switch
        {
            LogLevel.Trace => LogEventLevel.Verbose,
            LogLevel.Debug => LogEventLevel.Debug,
            LogLevel.Information => LogEventLevel.Information,
            LogLevel.Warning => LogEventLevel.Warning,
            LogLevel.Error => LogEventLevel.Error,
            LogLevel.Critical => LogEventLevel.Fatal,
            LogLevel.None => (LogEventLevel) ((int) LogEventLevel.Fatal + 1),
            _ => throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null)
        };
    }
}
