using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValidLoaderShared.Utilities.Logging
{
    public class SerilogLogger : LoggerDecorator
    {
        public SerilogLogger(ILogger logger) : base(logger) { }

        protected override void CurrentLoggingBehavior<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var message = formatter != null ? formatter(state, exception) : state.ToString();
            Serilog.Log.ForContext<SerilogLogger>().Write(logLevel.ToSerilogEventLevel(), exception, message);
        }


    }

}
