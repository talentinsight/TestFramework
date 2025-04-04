using System;
using TestFramework.Core.Logger;

namespace TestFramework.Tests.Logger
{
    public static class LoggerFactory
    {
        public static ILogger CreateLogger(LoggerType type, string? filePath = null)
        {
            return type switch
            {
                LoggerType.Console => new ConsoleLogger(),
                LoggerType.File when !string.IsNullOrEmpty(filePath) => new FileLogger(filePath),
                LoggerType.Mock => new MockLogger(),
                _ => throw new ArgumentException($"Unsupported logger type: {type}")
            };
        }
    }
} 