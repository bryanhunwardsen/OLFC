using System;
using System.Collections.Generic;
using System.Text;
using static GenericApplicationLogger.Common;

namespace GenericApplicationLogger
{
    public interface ILogger
    {
        LogLevel DefaultLogLevel { get; set; }

        void LogMessage(string message);

        void LogMessage(string message, LogLevel logLevel);
    }
}
