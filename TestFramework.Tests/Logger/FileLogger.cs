using System;
using System.IO;
using TestFramework.Core.Logger;

namespace TestFramework.Tests.Logger
{
    public class FileLogger : ILogger
    {
        private readonly string _filePath;
        private LogLevel _currentLevel = LogLevel.Info;

        public FileLogger(string filePath)
        {
            _filePath = filePath;
        }

        public void Log(string message)
        {
            Log(message, LogLevel.Info);
        }

        public void Log(string message, LogLevel level)
        {
            if (level >= _currentLevel)
            {
                try
                {
                    File.AppendAllText(_filePath, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}{Environment.NewLine}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to write to log file: {ex.Message}");
                }
            }
        }

        public void SetLogLevel(LogLevel level)
        {
            _currentLevel = level;
        }
    }
} 