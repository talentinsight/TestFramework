using System;

namespace TestFramework.Core.Logger
{
    public class LoggerFactory
    {
        public static ILogger CreateLogger(LoggerType type, string filePath = null)
        {
            return type switch
            {
                LoggerType.Console => new ConsoleLogger(),
                LoggerType.File => new FileLogger(filePath ?? "log.txt"),
                LoggerType.Mock => new MockLogger(),
                _ => new ConsoleLogger()
            };
        }
    }
}

/* 
 * Defines the LoggerFactory class
 * Defines the CreateLogger function
 */     