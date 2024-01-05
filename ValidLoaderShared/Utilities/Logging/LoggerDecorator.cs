using Microsoft.Extensions.Logging;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValidLoaderShared.Utilities.Logging
{
    public abstract class LoggerDecorator : LoggerBase, ISimplifiedLogger
    {
        protected readonly ILogger WrappedLogger;

        public LoggerDecorator(ILogger logger)
        {
            WrappedLogger = logger;
        }

        public override void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            CurrentLoggingBehavior(logLevel, eventId, state, exception, formatter);
            WrappedLogger?.Log(logLevel, eventId, state, exception, formatter);
        }

        public virtual void Log(string message, LogLevel logLevel = LogLevel.Information)
        {
            Log(logLevel, new EventId(), message, null, (state, exception) => state.ToString());
        }
        protected abstract void CurrentLoggingBehavior<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter);
        // Other methods (IsEnabled, BeginScope) would similarly delegate to WrappedLogger
    }

}
