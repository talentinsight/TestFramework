using System;
using TestFramework.Core.Logger;

namespace TestFramework.Tests.Logger
{
    public class ConsoleLogger : ILogger
    {
        private LogLevel _currentLevel = LogLevel.Info;

        public void Log(string message, LogLevel level)
        {
            if (level >= _currentLevel)
            {
                var logMessage = $"[{level.ToString().ToUpper()}] {message}";
                Console.WriteLine(logMessage);
            }
        }

        public void Log(string message)
        {
            Log(message, LogLevel.Info);
        }

        public void SetLogLevel(LogLevel level)
        {
            _currentLevel = level;
        }
    }
} 