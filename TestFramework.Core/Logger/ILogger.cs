using System;

namespace TestFramework.Core.Logger
{
    /// <summary>
    /// Interface for logging functionality
    /// </summary>
<<<<<<< HEAD
    public interface ILogger
=======
    public interface ILogger : IDisposable
>>>>>>> df66c302549408ea17e5338bbce861a452d6d404
    {
        /// <summary>
        /// Logs a message with the specified log level
        /// </summary>
<<<<<<< HEAD
        /// <param name="message">The message to log</param>
        /// <param name="level">The log level</param>
        void Log(string message, LogLevel level);

        /// <summary>
        /// Logs a message with default Info level
        /// </summary>
        /// <param name="message">The message to log</param>
        void Log(string message);

        /// <summary>
        /// Sets the current log level
        /// </summary>
        /// <param name="level">The log level to set</param>
        void SetLogLevel(LogLevel level);
=======
        /// <param name="level">The log level</param>
        /// <param name="message">The message to log</param>
        void Log(LogLevel level, string message);

        /// <summary>
        /// Logs a message with the specified log level and exception
        /// </summary>
        /// <param name="level">The log level</param>
        /// <param name="message">The message to log</param>
        /// <param name="exception">The exception to log</param>
        void Log(LogLevel level, string message, Exception exception);

        /// <summary>
        /// Logs a message with Info level
        /// </summary>
        /// <param name="message">The message to log</param>
        void Log(string message) => Log(LogLevel.Info, message);
>>>>>>> df66c302549408ea17e5338bbce861a452d6d404
    }
}

/* 
 * Defines the ILogger interface
 * Defines the Log(string message) function
 */ 