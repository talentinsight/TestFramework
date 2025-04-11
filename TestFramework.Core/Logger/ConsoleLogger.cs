using System;

namespace TestFramework.Core.Logger
{
    /// <summary>
    /// Logger implementation that writes to the console
    /// </summary>
    public class ConsoleLogger : ILogger
    {
        private bool _disposed;

        /// <summary>
        /// Logs a message with the specified log level
        /// </summary>
        /// <param name="level">The log level</param>
        /// <param name="message">The message to log</param>
        public void Log(LogLevel level, string message)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(ConsoleLogger));

            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            string levelStr = level switch
            {
                LogLevel.Debug => "[Debug]",
                LogLevel.Info => "[Info]",
                LogLevel.Warning => "[Warning]",
                LogLevel.Error => "[Error]",
                _ => "[Unknown]"
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

            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            string levelStr = level switch
            {
                LogLevel.Debug => "[Debug]",
                LogLevel.Info => "[Info]",
                LogLevel.Warning => "[Warning]",
                LogLevel.Error => "[Error]",
                _ => "[Unknown]"
            };

            string logMessage = $"{timestamp} {levelStr} {message}";
            logMessage += $"{Environment.NewLine}Exception: {exception.GetType().Name}: {exception.Message}";
            if (exception.StackTrace != null)
            {
                logMessage += $"{Environment.NewLine}Stack Trace: {exception.StackTrace}";
            }

            Console.WriteLine(logMessage);
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
