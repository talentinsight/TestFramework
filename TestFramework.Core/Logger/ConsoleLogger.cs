using System;

namespace TestFramework.Core.Logger
{
    /// <summary>
    /// Logger implementation that writes to the console
    /// </summary>
    public class ConsoleLogger : ILogger
    {
        /// <summary>
        /// Logs a message to the console with default log level
        /// </summary>
        /// <param name="message">The message to log</param>
        public void Log(string message)
        {
            Log(message, LogLevel.Info);
        }

        /// <summary>
        /// Logs a message to the console with specified log level
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="level">The log level</param>
        public void Log(string message, LogLevel level)
        {
            var prefix = level switch
            {
                LogLevel.Info => "[INFO]",
                LogLevel.Warning => "[WARNING]",
                LogLevel.Error => "[ERROR]",
                _ => string.Empty
            };
            Console.WriteLine($"{prefix} {message}");
        }
    }
}
