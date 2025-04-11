using System;
using TestFramework.Core.Logger;

namespace TestFramework.Core.Tests
{
    /// <summary>
    /// Base class for all tests
    /// </summary>
    public abstract class TestBase
    {
        protected ILogger Logger { get; private set; }

        /// <summary>
        /// Initializes a new instance of the TestBase class
        /// </summary>
        /// <param name="loggerType">Type of logger to use</param>
        protected TestBase(LoggerType loggerType = LoggerType.Console)
        {
            Logger = CreateLogger(loggerType);
        }

        /// <summary>
        /// Creates a logger of the specified type
        /// </summary>
        /// <param name="loggerType">Type of logger to create</param>
        /// <returns>The created logger</returns>
        protected virtual ILogger CreateLogger(LoggerType loggerType)
        {
            return loggerType switch
            {
                LoggerType.Console => new ConsoleLogger(),
                LoggerType.File => new FileLogger("test.log"),
                LoggerType.Mock => new MockLogger(),
                _ => new ConsoleLogger()
            };
        }

        /// <summary>
        /// Sets up the test
        /// </summary>
        protected abstract void Setup();

        /// <summary>
        /// Runs the test
        /// </summary>
        protected abstract void RunTest();

        /// <summary>
        /// Tears down the test
        /// </summary>
        protected abstract void TearDown();

        /// <summary>
        /// Executes the test
        /// </summary>
        public void Execute()
        {
            try
            {
                Setup();
                RunTest();
            }
            finally
            {
                TearDown();
            }
        }
    }

    /// <summary>
    /// Enum for logger types
    /// </summary>
    public enum LoggerType
    {
        Console,
        File,
        Mock
    }
} 