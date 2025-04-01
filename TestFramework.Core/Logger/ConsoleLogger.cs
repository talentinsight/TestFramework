using System;

namespace TestFramework.Core.Logger
{
    public class ConsoleLogger : ILogger
    {
        public void Log(string message)
        {
            Log(message, LogLevel.Info);
        }

        public void Log(string message, LogLevel level)
        {
            Console.WriteLine($"[Console] [{level}] {DateTime.Now:HH:mm:ss} - {message}");
        }
    }
}
