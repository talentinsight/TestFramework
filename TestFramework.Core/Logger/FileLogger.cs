using System;
using System.IO;
using System.Threading;

namespace TestFramework.Core.Logger
{
    /// <summary>
    /// Logger implementation that writes to a file
    /// </summary>
    public class FileLogger : ILogger, IDisposable
    {
        private readonly string _filePath;
        private readonly object _lock = new object();
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the FileLogger class
        /// </summary>
        /// <param name="filePath">The path to the log file</param>
        public FileLogger(string filePath)
        {
            _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
        }

        /// <summary>
        /// Logs a message with the specified log level
        /// </summary>
        /// <param name="level">The log level</param>
        /// <param name="message">The message to log</param>
        public void Log(LogLevel level, string message)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(FileLogger));

            string logMessage = FormatLogMessage(level, message, null);
            WriteToFile(logMessage);
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
                throw new ObjectDisposedException(nameof(FileLogger));

            string logMessage = FormatLogMessage(level, message, exception);
            WriteToFile(logMessage);
        }

        private string FormatLogMessage(LogLevel level, string message, Exception? exception)
        {
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

            if (exception != null)
            {
                logMessage += $"{Environment.NewLine}Exception: {exception.GetType().Name}: {exception.Message}";
                if (exception.StackTrace != null)
                {
                    logMessage += $"{Environment.NewLine}Stack Trace: {exception.StackTrace}";
                }
            }

            return logMessage;
        }

        private void WriteToFile(string message)
        {
            lock (_lock)
            {
                File.AppendAllText(_filePath, message + Environment.NewLine);
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
            }
        }
    }
}

/* 
 * Defines the FileLogger class
 * Defines the Log(string message) function
 */     