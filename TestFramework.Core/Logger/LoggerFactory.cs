using System;

namespace TestFramework.Core.Logger
{
    /// <summary>
    /// Factory class for creating logger instances
    /// </summary>
    public static class LoggerFactory
    {
        /// <summary>
        /// Creates a logger instance of the specified type
        /// </summary>
        /// <param name="type">The type of logger to create</param>
        /// <param name="filePath">The file path for file-based loggers</param>
        /// <returns>A new logger instance</returns>
        public static ILogger CreateLogger(LoggerType type, string? filePath = null)
        {
            return type switch
            {
                LoggerType.Console => new ConsoleLogger(),
                LoggerType.File => new FileLogger(filePath ?? throw new ArgumentNullException(nameof(filePath))),
                LoggerType.Mock => new MockLogger(),
                _ => throw new ArgumentException($"Unknown logger type: {type}", nameof(type))
            };
        }
    }
}

/* 
 * Defines the LoggerFactory class
 * Defines the CreateLogger function
 */     