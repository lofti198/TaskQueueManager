using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValidLoaderShared.Utilities.Logging
{
    public abstract class LoggerBase : ILogger
    {
        public abstract void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter);

        public virtual bool IsEnabled(LogLevel logLevel)
        {
            // Implement logic to determine if the given log level is enabled
            return true;
        }

        public virtual IDisposable BeginScope<TState>(TState state)
        {
            // Implement scope management if necessary
            return null;
        }
    }


}
