using System;
using System.IO;

namespace TestFramework.Core.Logger
{
    public class FileLogger : ILogger
    {
        private readonly string _filePath;

        public FileLogger(string filePath)
        {
            _filePath = filePath;
        }

        public void Log(string message)
        {
            Log(message, LogLevel.Info); // Default level
        }

        public void Log(string message, LogLevel level)
        {
            var logLine = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}";
            File.AppendAllText(_filePath, logLine + Environment.NewLine);
        }
    }
}

/* 
 * Defines the FileLogger class
 * Defines the Log(string message) function
 */     