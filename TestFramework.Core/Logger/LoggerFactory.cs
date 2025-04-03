using System;
using TestFramework.Core.Interfaces;

namespace TestFramework.Core.Logger
{
    /// <summary>
    /// Factory class for creating logger instances
    /// </summary>
    public static class LoggerFactory
    {
        /// <summary>
        /// Creates a logger instance based on the specified type
        /// </summary>
        /// <param name="loggerType">Type of logger to create</param>
        /// <returns>Logger instance</returns>
        public static ILogger CreateLogger(LoggerType loggerType)
        {
            return loggerType switch
            {
                LoggerType.Console => new ConsoleLogger(),
                LoggerType.File => new FileLogger(),
                LoggerType.Database => new DatabaseLogger(),
                _ => throw new ArgumentException($"Invalid logger type: {loggerType}", nameof(loggerType))
            };
        }
    }
}

/* 
 * Defines the LoggerFactory class
 * Defines the CreateLogger function
 */     