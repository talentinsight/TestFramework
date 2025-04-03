using System;
using System.Collections.Generic;

namespace TestFramework.Core.Logger
{
    /// <summary>
    /// Mock logger implementation for testing purposes
    /// </summary>
    public class MockLogger : ILogger
    {
        /// <summary>
        /// Gets the list of logged messages
        /// </summary>
        public List<string> LoggedMessages { get; } = new List<string>();

        /// <summary>
        /// Logs a message with default log level
        /// </summary>
        /// <param name="message">The message to log</param>
        public void Log(string message)
        {
            Log(message, LogLevel.Info);
        }

        /// <summary>
        /// Logs a message with specified log level
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="level">The log level</param>
        public void Log(string message, LogLevel level)
        {
            LoggedMessages.Add($"[{level}] {message}");
        }

        /// <summary>
        /// Clears all logged messages
        /// </summary>
        public void Clear()
        {
            LoggedMessages.Clear();
        }
    }
}

/* 
 * Defines the MockLogger class
 * Defines the Log(string message) function
 */         