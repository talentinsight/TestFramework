using System;
using System.Collections.Generic;

namespace TestFramework.Core.Logger
{
    public class MockLogger : ILogger
    {
        private readonly List<string> _logs = new List<string>();

        public IReadOnlyList<string> Logs => _logs;

        public void Log(string message)
        {
            Log(message, LogLevel.Info);
        }

        public void Log(string message, LogLevel level)
        {
            var logMessage = $"[Mock] [{level}] {DateTime.Now:HH:mm:ss} - {message}";
            _logs.Add(logMessage);
        }

        public void Clear()
        {
            _logs.Clear();
        }
    }
}

/* 
 * Defines the MockLogger class
 * Defines the Log(string message) function
 */         