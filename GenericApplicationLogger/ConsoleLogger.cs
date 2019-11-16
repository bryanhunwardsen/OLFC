using System;
using static GenericApplicationLogger.Common;

namespace GenericApplicationLogger
{
    public class ConsoleLogger : ILogger
    {
        public LogLevel DefaultLogLevel { get; set; }

        public ConsoleLogger()
        {
            DefaultLogLevel = LogLevel.DEBUG;
        }

        public ConsoleLogger(LogLevel logLevel)
        {
            DefaultLogLevel = logLevel;
        }

        public void LogMessage(string message)
        {
            MessageWriter(message, DefaultLogLevel);
        }

        public void LogMessage(string message, LogLevel logLevel)
        {
            MessageWriter(message, logLevel);
        }

        private void MessageWriter(string message, LogLevel logLevel)
        {
            if (logLevel >= DefaultLogLevel)
                Console.WriteLine($"{DateTime.UtcNow.ToString("yyyyMMddTH:mm:ss:fffffff")}UTC|{logLevel}|{message}");
        }
    }
}
