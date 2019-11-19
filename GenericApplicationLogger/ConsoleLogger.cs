using System;
using static GenericApplicationLogger.Common;

namespace GenericApplicationLogger
{
    /// <summary>
    /// Simple Console Logger
    /// </summary>
    public class ConsoleLogger : ILogger
    {
        /// <inheritdoc/>
        public LogLevel DefaultLogLevel { get; set; }

        /// <summary>
        /// Console Logger with default Debug LogLevel
        /// </summary>
        public ConsoleLogger()
        {
            DefaultLogLevel = LogLevel.DEBUG;
        }

        /// <summary>
        /// Console Logger with the definied default Log Level
        /// </summary>
        /// <param name="logLevel">Log level to set as default</param>
        public ConsoleLogger(LogLevel logLevel)
        {
            DefaultLogLevel = logLevel;
        }

        /// <inheritdoc/>
        public void LogMessage(string message)
        {
            MessageWriter(message, DefaultLogLevel);
        }

        /// <inheritdoc/>
        public void LogMessage(string message, LogLevel logLevel)
        {
            MessageWriter(message, logLevel);
        }

        /// <summary>
        /// Formats the log message
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="logLevel">Level to log Message at</param>
        private void MessageWriter(string message, LogLevel logLevel)
        {
            if (logLevel >= DefaultLogLevel)
                Console.WriteLine($"{DateTime.UtcNow.ToString("yyyyMMddTH:mm:ss:fffffff")}UTC|{logLevel}|{message}");
        }
    }
}
