namespace TestFramework.Core.Logger
{
    public interface ILogger
    {
        void Log(string message);
        void Log(string message, LogLevel level);
    }
}

/* 
 * Defines the ILogger interface
 * Defines the Log(string message) function
 */ 