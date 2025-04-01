using NUnit.Framework;
using TestFramework.Core.Logger;
using System;
using System.IO;

namespace TestFramework.Tests.Logger
{
    public class LoggerTests
    {
        private string _testLogPath;

        [SetUp]
        public void Setup()
        {
            _testLogPath = Path.Combine(Path.GetTempPath(), "test_log.txt");
            if (File.Exists(_testLogPath))
            {
                File.Delete(_testLogPath);
            }
        }

        [TestCase(LoggerType.Console, LogLevel.Info)]
        [TestCase(LoggerType.Console, LogLevel.Warning)]
        [TestCase(LoggerType.Console, LogLevel.Error)]
        public void ConsoleLogger_ShouldLog_WithDifferentLevels(LoggerType loggerType, LogLevel level)
        {
            // Arrange
            var logger = LoggerFactory.CreateLogger(loggerType);

            // Act
            logger.Log("Test log message", level);

            // Assert (Console output is not directly assertable, so we assume it runs without exception)
            Assert.Pass("Console log test executed successfully.");
        }

        [TestCase(LogLevel.Info)]
        [TestCase(LogLevel.Warning)]
        [TestCase(LogLevel.Error)]
        public void FileLogger_ShouldLog_WithDifferentLevels(LogLevel level)
        {
            // Arrange
            var logger = LoggerFactory.CreateLogger(LoggerType.File, _testLogPath);

            // Act
            logger.Log("Test log message", level);

            // Assert
            var logExists = File.Exists(_testLogPath);
            Assert.IsTrue(logExists, "Log file should exist.");
            var fileContents = File.ReadAllText(_testLogPath);
            Assert.IsTrue(fileContents.Contains($"[{level}]"), "Log file should contain the correct log level.");
        }
    }
}

/* 
 * Defines the LoggerTests class
 * Defines ConsoleLogger_ShouldLog_WithDifferentLevels function
 * Defines FileLogger_ShouldLog_WithDifferentLevels function
 */     
    