using System;
using System.IO;

namespace TestFramework.Core.Logger
{
    /// <summary>
    /// Logger implementation that writes to a file
    /// </summary>
    public class FileLogger : ILogger
    {
        private readonly string _filePath;

        /// <summary>
        /// Initializes a new instance of the FileLogger class
        /// </summary>
        /// <param name="filePath">Path to the log file</param>
        public FileLogger(string filePath)
        {
            _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
        }

        /// <summary>
        /// Logs a message to the file with default log level
        /// </summary>
        /// <param name="message">The message to log</param>
        public void Log(string message)
        {
            Log(message, LogLevel.Info);
        }

        /// <summary>
        /// Logs a message to the file with specified log level
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="level">The log level</param>
        public void Log(string message, LogLevel level)
        {
            var logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}";
            File.AppendAllText(_filePath, logMessage + Environment.NewLine);
        }
    }
}

/* 
 * Defines the FileLogger class
 * Defines the Log(string message) function
 */     