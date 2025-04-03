using System;
using System.Collections.Generic;

namespace TestFramework.Core.Logger
{
    /// <summary>
    /// Mock logger implementation for testing purposes
    /// </summary>
    public class MockLogger : ILogger
    {
        private readonly List<string> _logs = new List<string>();
        private readonly List<string> _errorMessages = new List<string>();
        private readonly List<string> _logMessages = new List<string>();

        /// <summary>
        /// Gets the list of all log messages
        /// </summary>
        public IReadOnlyList<string> Logs => _logs;

        /// <summary>
        /// Gets the list of error messages
        /// </summary>
        public IReadOnlyList<string> ErrorMessages => _errorMessages;

        /// <summary>
        /// Gets the list of log messages
        /// </summary>
        public IReadOnlyList<string> LogMessages => _logMessages;

        /// <summary>
        /// Logs a message with the specified log level
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="level">The log level</param>
        public void Log(string message, LogLevel level = LogLevel.Info)
        {
            _logs.Add($"[{level}] {message}");
            _logMessages.Add(message);

            if (level == LogLevel.Error)
            {
                _errorMessages.Add(message);
            }
        }

        /// <summary>
        /// Logs a message with default Info level
        /// </summary>
        /// <param name="message">The message to log</param>
        public void Log(string message)
        {
            Log(message, LogLevel.Info);
        }

        /// <summary>
        /// Clears all logged messages
        /// </summary>
        public void Clear()
        {
            _logs.Clear();
            _errorMessages.Clear();
            _logMessages.Clear();
        }
    }
}

/* 
 * Defines the MockLogger class
 * Defines the Log(string message) function
 */         