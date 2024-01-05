using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValidLoaderShared.Utilities.Logging
{
    public class ConsoleLogger : LoggerDecorator
    {
        private readonly bool _includeStackTrace;
        public ConsoleLogger(ILogger logger, bool includeStackTrace) : base(logger)
        {
            _includeStackTrace = includeStackTrace;
        }

        protected override void CurrentLoggingBehavior<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var message = formatter != null ? formatter(state, exception) : state.ToString();
            Console.WriteLine($"[ConsoleLogger] {DateTime.Now} [{logLevel}] {message}");
            if (exception != null && _includeStackTrace)
            {
                Console.WriteLine($"Exception: {exception}");
            }
        }

    }

}
