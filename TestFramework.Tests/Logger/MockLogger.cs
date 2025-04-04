using System;
using System.Collections.Generic;
using TestFramework.Core.Logger;

namespace TestFramework.Tests.Logger
{
    /// <summary>
    /// Mock logger implementation for testing purposes
    /// </summary>
    public class MockLogger : ILogger
    {
        private readonly List<string> _logs = new();
        private LogLevel _currentLogLevel = LogLevel.Info;
        private readonly List<string> _errorMessages = new();

        /// <summary>
        /// Gets the list of all log messages
        /// </summary>
        public List<string> Logs => new List<string>(_logs);

        /// <summary>
        /// Gets the list of all error messages
        /// </summary>
        public List<string> ErrorMessages => new List<string>(_errorMessages);

        /// <summary>
        /// Gets or sets the current log level
        /// </summary>
        public LogLevel CurrentLogLevel
        {
            get => _currentLogLevel;
            set => _currentLogLevel = value;
        }

        /// <summary>
        /// Logs a message with the specified log level
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="level">The log level</param>
        public void Log(string message, LogLevel level)
        {
            if (level >= _currentLogLevel)
            {
                var logMessage = $"[{level.ToString().ToUpper()}] {message}";
                _logs.Add(logMessage);
                if (level == LogLevel.Error)
                {
                    _errorMessages.Add(message);
                }
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
        /// Sets the current log level
        /// </summary>
        /// <param name="level">The log level to set</param>
        public void SetLogLevel(LogLevel level)
        {
            _currentLogLevel = level;
        }

        /// <summary>
        /// Clears all logged messages
        /// </summary>
        public void Clear()
        {
            _logs.Clear();
            _errorMessages.Clear();
        }

        /// <summary>
        /// Clears all logged messages
        /// </summary>
        public void ClearLogs()
        {
            Clear();
        }

        /// <summary>
        /// Gets the most recent logs
        /// </summary>
        /// <param name="count">Number of logs to return</param>
        /// <returns>List of recent logs</returns>
        public List<string> GetRecentLogs(int count)
        {
            if (count <= 0)
            {
                return new List<string>();
            }

            var startIndex = Math.Max(0, _logs.Count - count);
            return _logs.GetRange(startIndex, Math.Min(count, _logs.Count - startIndex));
        }
    }
} 