using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValidLoaderShared.Utilities.Logging
{
    public interface ISimplifiedLogger : ILogger
    {
        void Log(string message, LogLevel logLevel = LogLevel.Information);
    }
}
