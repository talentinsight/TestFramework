using System;

namespace TestFramework.Core.Logger
{
    /// <summary>
    /// Interface for logging functionality
    /// </summary>
    public interface ILogger : IDisposable
    {
        /// <summary>
        /// Logs a message with the specified log level
        /// </summary>
        /// <param name="level">The log level</param>
        /// <param name="message">The message to log</param>
        void Log(LogLevel level, string message);

        /// <summary>
        /// Logs a message with the specified log level and exception
        /// </summary>
        /// <param name="level">The log level</param>
        /// <param name="message">The message to log</param>
        /// <param name="exception">The exception to log</param>
        void Log(LogLevel level, string message, Exception exception);

        /// <summary>
        /// Logs a message with Info level
        /// </summary>
        /// <param name="message">The message to log</param>
        void Log(string message) => Log(LogLevel.Info, message);
    }
}

/* 
 * Defines the ILogger interface
 * Defines the Log(string message) function
 */ 