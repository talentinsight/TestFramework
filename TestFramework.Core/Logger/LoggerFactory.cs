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
        /// <param name="type">The type of logger to create</param>
        /// <param name="filePath">The file path for file logger (optional)</param>
        /// <returns>A logger instance</returns>
        /// <exception cref="ArgumentException">Thrown when an invalid logger type is specified</exception>
        /// <exception cref="ArgumentNullException">Thrown when filePath is null for File logger type</exception>
        public static ILogger CreateLogger(LoggerType type, string? filePath = null)
        {
            return type switch
            {
                LoggerType.Console => new ConsoleLogger(),
                LoggerType.File => new FileLogger(filePath ?? throw new ArgumentNullException(nameof(filePath))),
                LoggerType.Mock => new MockLogger(),
                _ => throw new ArgumentException($"Invalid logger type: {type}", nameof(type))
            };
        }
    }
}

/* 
 * Defines the LoggerFactory class
 * Defines the CreateLogger function
 */     