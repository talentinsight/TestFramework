using System;
using System.Collections.Generic;

namespace TestFramework.Core.Logger
{
    /// <summary>
    /// Mock logger implementation for testing
    /// </summary>
    public class MockLogger : ILogger
    {
        private readonly List<LogEntry> _logEntries = new List<LogEntry>();
        private bool _disposed;
        private LogLevel _currentLogLevel = LogLevel.Info;

        public IReadOnlyList<LogEntry> LogEntries => _logEntries;

        /// <summary>
        /// Logs a message with the specified log level
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="level">The log level</param>
        public void Log(string message, LogLevel level)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(MockLogger));

            if (level < _currentLogLevel)
                return;

            _logEntries.Add(new LogEntry(level, message, null));
        }

        /// <summary>
        /// Logs a message with the specified log level and exception
        /// </summary>
        /// <param name="level">The log level</param>
        /// <param name="message">The message to log</param>
        /// <param name="exception">The exception to log</param>
        public void Log(LogLevel level, string message, Exception exception)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(MockLogger));

            if (level < _currentLogLevel)
                return;

            _logEntries.Add(new LogEntry(level, message, exception));
        }

        /// <summary>
        /// Logs a message with Info level
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

        public void Clear()
        {
            _logEntries.Clear();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _logEntries.Clear();
                }
                _disposed = true;
            }
        }
    }

    public class LogEntry
    {
        public LogLevel Level { get; }
        public string Message { get; }
        public Exception? Exception { get; }
        public DateTime Timestamp { get; }

        public LogEntry(LogLevel level, string message, Exception? exception = null)
        {
            Level = level;
            Message = message;
            Exception = exception;
            Timestamp = DateTime.Now;
        }
    }
}

/* 
 * Defines the MockLogger class
 * Defines the Log(string message) function
 */         