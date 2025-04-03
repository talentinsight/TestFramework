using System;

namespace TestFramework.Core.Logger
{
    /// <summary>
    /// Interface for logging functionality
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Logs a message with default log level
        /// </summary>
        /// <param name="message">The message to log</param>
        void Log(string message);

        /// <summary>
        /// Logs a message with specified log level
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="level">The log level</param>
        void Log(string message, LogLevel level);
    }
}

/* 
 * Defines the ILogger interface
 * Defines the Log(string message) function
 */ 