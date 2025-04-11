<<<<<<< HEAD
=======
using System.Collections.Generic;
using Xunit;
using TestFramework.Core.Logger;
>>>>>>> d14476f86062bbdacd7e5bd7c4a9cb8565e91e68
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestFramework.Core.Logger;

namespace TestFramework.Tests.Logger
{
    [TestClass]
    public class LoggerTests
    {
<<<<<<< HEAD
        private string _logFilePath = null!;
        private ILogger _logger = null!;
        private StringWriter _consoleOutput = null!;
        private TextWriter _originalConsoleOutput = null!;

        [TestInitialize]
        public void Setup()
        {
            _logFilePath = Path.Combine(Path.GetTempPath(), $"test_log_{Guid.NewGuid()}.txt");
            _logger = new FileLogger(_logFilePath);

            // Capture console output
            _originalConsoleOutput = Console.Out;
            _consoleOutput = new StringWriter();
            Console.SetOut(_consoleOutput);
        }

        [TestCleanup]
        public void Cleanup()
        {
            // Restore console output
            Console.SetOut(_originalConsoleOutput);
            _consoleOutput.Dispose();

            // Clean up log file
            if (File.Exists(_logFilePath))
            {
                File.Delete(_logFilePath);
            }
        }

        [TestMethod]
        public void Log_InfoMessage_LogsCorrectly()
        {
            // Arrange
            const string message = "Test info message";

            // Act
            _logger.Log(LogLevel.Info, message);

            // Assert
            string logContent = File.ReadAllText(_logFilePath);
            Assert.IsTrue(logContent.Contains(message), "Log message not found in file");
            Assert.IsTrue(logContent.Contains("[Info]"), "Log level not found in file");
        }

        [TestMethod]
        public void Log_WarningMessage_LogsCorrectly()
        {
            // Arrange
            const string message = "Test warning message";

            // Act
            _logger.Log(LogLevel.Warning, message);

            // Assert
            string logContent = File.ReadAllText(_logFilePath);
            Assert.IsTrue(logContent.Contains(message), "Log message not found in file");
            Assert.IsTrue(logContent.Contains("[Warning]"), "Log level not found in file");
        }

        [TestMethod]
        public void Log_ErrorMessage_LogsCorrectly()
        {
            // Arrange
            const string message = "Test error message";

            // Act
            _logger.Log(LogLevel.Error, message);

            // Assert
            string logContent = File.ReadAllText(_logFilePath);
            Assert.IsTrue(logContent.Contains(message), "Log message not found in file");
            Assert.IsTrue(logContent.Contains("[Error]"), "Log level not found in file");
        }

        [TestMethod]
        public void Log_WithException_LogsExceptionDetails()
        {
            // Arrange
            const string message = "Test exception message";
            var exception = new InvalidOperationException("Test exception");

            // Act
            _logger.Log(LogLevel.Error, message, exception);

            // Assert
            string logContent = File.ReadAllText(_logFilePath);
            Assert.IsTrue(logContent.Contains(message), "Log message not found in file");
            Assert.IsTrue(logContent.Contains(exception.Message), "Exception message not found in file");
            Assert.IsTrue(logContent.Contains(exception.GetType().Name), "Exception type not found in file");
        }

        [TestMethod]
        public async Task Log_ConcurrentLogging_LogsAllMessages()
        {
            // Arrange
            const int messageCount = 100;
            var tasks = new Task[messageCount];

            // Act
            for (int i = 0; i < messageCount; i++)
            {
                int messageNumber = i;
                tasks[i] = Task.Run(() => _logger.Log(LogLevel.Info, $"Message {messageNumber}"));
            }
            await Task.WhenAll(tasks);

            // Assert
            string logContent = File.ReadAllText(_logFilePath);
            for (int i = 0; i < messageCount; i++)
            {
                Assert.IsTrue(logContent.Contains($"Message {i}"), $"Message {i} not found in log");
            }
=======
        private readonly MockLogger _logger;

        public LoggerTests()
        {
            _logger = new MockLogger();
        }

        [Theory]
        [InlineData(LogLevel.Debug, "Debug message")]
        [InlineData(LogLevel.Info, "Info message")]
        [InlineData(LogLevel.Warning, "Warning message")]
        [InlineData(LogLevel.Error, "Error message")]
        public void Log_WithDifferentLevels_LogsCorrectly(LogLevel level, string message)
        {
            // Act
            _logger.Log(message, level);

            // Assert
            Assert.Contains(_logger.Logs, log => log.Contains($"[{level.ToString().ToUpper()}]") && log.Contains(message));
        }

        [Theory]
        [InlineData(LogLevel.Debug, LogLevel.Info)]
        [InlineData(LogLevel.Debug, LogLevel.Warning)]
        [InlineData(LogLevel.Debug, LogLevel.Error)]
        [InlineData(LogLevel.Info, LogLevel.Warning)]
        [InlineData(LogLevel.Info, LogLevel.Error)]
        [InlineData(LogLevel.Warning, LogLevel.Error)]
        public void Log_WhenLevelIsBelowCurrent_DoesNotLog(LogLevel messageLevel, LogLevel currentLevel)
        {
            // Arrange
            _logger.CurrentLogLevel = currentLevel;

            // Act
            _logger.Log("Test message", messageLevel);

            // Assert
            Assert.Empty(_logger.Logs);
        }

        [Fact]
        public void Log_WithoutLevel_UsesInfoLevel()
        {
            // Act
            _logger.Log("Test message");

            // Assert
            Assert.Contains(_logger.Logs, log => log.Contains("[INFO]") && log.Contains("Test message"));
        }

        [Fact]
        public void ClearLogs_RemovesAllLogs()
        {
            // Arrange
            _logger.Log("Test message 1");
            _logger.Log("Test message 2");

            // Act
            _logger.ClearLogs();

            // Assert
            Assert.Empty(_logger.Logs);
>>>>>>> d14476f86062bbdacd7e5bd7c4a9cb8565e91e68
        }
    }
}

/* 
 * Defines the LoggerTests class
<<<<<<< HEAD
 * Defines Log_InfoMessage_LogsCorrectly function
 * Defines Log_WarningMessage_LogsCorrectly function
 * Defines Log_ErrorMessage_LogsCorrectly function
 * Defines Log_WithException_LogsExceptionDetails function
 * Defines Log_ConcurrentLogging_LogsAllMessages function
=======
 * Defines Log_WithDifferentLevels_LogsCorrectly function
 * Defines Log_WhenLevelIsBelowCurrent_DoesNotLog function
 * Defines Log_WithoutLevel_UsesInfoLevel function
 * Defines ClearLogs_RemovesAllLogs function
>>>>>>> d14476f86062bbdacd7e5bd7c4a9cb8565e91e68
 */     
    