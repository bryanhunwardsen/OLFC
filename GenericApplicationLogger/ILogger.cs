using static GenericApplicationLogger.Common;

namespace GenericApplicationLogger
{
    /// <summary>
    /// Generic simple logging interface
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Default Log Level
        /// Will log all events to the default level unless otherwise specified
        /// Only log event equal to or more severe to the default logging level will be written at runtime
        /// </summary>
        LogLevel DefaultLogLevel { get; set; }

        /// <summary>
        /// Log message at the default log level
        /// </summary>
        /// <param name="message">Message to log</param>
        void LogMessage(string message);

        /// <summary>
        /// Log message at the indicated log level
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="logLevel">Level to log message at</param>
        void LogMessage(string message, LogLevel logLevel);
    }
}
