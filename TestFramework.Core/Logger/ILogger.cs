using System;

namespace TestFramework.Core.Logger
{
    /// <summary>
    /// Interface for logging functionality
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Logs a message with the specified log level
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="level">The log level</param>
        void Log(string message, LogLevel level);

        /// <summary>
        /// Logs a message with default Info level
        /// </summary>
        /// <param name="message">The message to log</param>
        void Log(string message);

        /// <summary>
        /// Sets the current log level
        /// </summary>
        /// <param name="level">The log level to set</param>
        void SetLogLevel(LogLevel level);
    }
}

/* 
 * Defines the ILogger interface
 * Defines the Log(string message) function
 */ 