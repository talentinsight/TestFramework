using System;

namespace TestFramework.Core.Logger
{
    /// <summary>
    /// Logger implementation that writes to the console
    /// </summary>
    public class ConsoleLogger : ILogger
    {
        private bool _disposed;
        private LogLevel _currentLogLevel = LogLevel.Info;

        /// <summary>
        /// Logs a message with the specified log level
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="level">The log level</param>
        public void Log(string message, LogLevel level)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(ConsoleLogger));

            if (level < _currentLogLevel)
                return;

            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            string levelStr = level switch
            {
                LogLevel.Trace => "[TRACE]",
                LogLevel.Debug => "[DEBUG]",
                LogLevel.Info => "[INFO]",
                LogLevel.Warning => "[WARNING]",
                LogLevel.Error => "[ERROR]",
                LogLevel.Fatal => "[FATAL]",
                LogLevel.Critical => "[CRITICAL]",
                _ => "[UNKNOWN]"
            };

            Console.WriteLine($"{timestamp} {levelStr} {message}");
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
                throw new ObjectDisposedException(nameof(ConsoleLogger));
                
            if (level < _currentLogLevel)
                return;

            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            string levelStr = level switch
            {
                LogLevel.Trace => "[TRACE]",
                LogLevel.Debug => "[DEBUG]",
                LogLevel.Info => "[INFO]",
                LogLevel.Warning => "[WARNING]",
                LogLevel.Error => "[ERROR]",
                LogLevel.Fatal => "[FATAL]",
                LogLevel.Critical => "[CRITICAL]",
                _ => "[UNKNOWN]"
            };

            string logMessage = $"{timestamp} {levelStr} {message}";
            logMessage += $"{Environment.NewLine}Exception: {exception.GetType().Name}: {exception.Message}";
            if (exception.StackTrace != null)
            {
                logMessage += $"{Environment.NewLine}Stack Trace: {exception.StackTrace}";
            }

            Console.WriteLine(logMessage);
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
                    // Clean up managed resources
                }
                _disposed = true;
            }
        }
    }
}
