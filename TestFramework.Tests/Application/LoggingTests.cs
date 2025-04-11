using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestFramework.Core.Application;
using TestFramework.Core.Logger;

namespace TestFramework.Tests.Application
{
    [TestClass]
    public class LoggingTests
    {
        private string _logFilePath = null!;
        private CppApplication _app = null!;

        [TestInitialize]
        public void Setup()
        {
            _logFilePath = Path.Combine(Path.GetTempPath(), $"test_log_{Guid.NewGuid()}.txt");
            _app = new CppApplication();
            _app.Logger = new FileLogger(_logFilePath);
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (File.Exists(_logFilePath))
            {
                File.Delete(_logFilePath);
            }
        }

        [TestMethod]
        public void LogMessage_InfoLevel_LogsCorrectly()
        {
            // Arrange
            var message = "Test info message";

            // Act
            _app.Logger.Log(LogLevel.Info, message);

            // Assert
            Assert.IsTrue(File.Exists(_logFilePath));
            var logContent = File.ReadAllText(_logFilePath);
            Assert.IsTrue(logContent.Contains(message));
            Assert.IsTrue(logContent.Contains("[Info]"));
        }

        [TestMethod]
        public void LogMessage_WarningLevel_LogsCorrectly()
        {
            // Arrange
            var message = "Test warning message";

            // Act
            _app.Logger.Log(LogLevel.Warning, message);

            // Assert
            Assert.IsTrue(File.Exists(_logFilePath));
            var logContent = File.ReadAllText(_logFilePath);
            Assert.IsTrue(logContent.Contains(message));
            Assert.IsTrue(logContent.Contains("[Warning]"));
        }

        [TestMethod]
        public void LogMessage_ErrorLevel_LogsCorrectly()
        {
            // Arrange
            var message = "Test error message";
            var exception = new Exception("Test exception");

            // Act
            _app.Logger.Log(LogLevel.Error, message, exception);

            // Assert
            Assert.IsTrue(File.Exists(_logFilePath));
            var logContent = File.ReadAllText(_logFilePath);
            Assert.IsTrue(logContent.Contains(message));
            Assert.IsTrue(logContent.Contains("[Error]"));
            Assert.IsTrue(logContent.Contains(exception.Message));
        }

        [TestMethod]
        public void LogMessage_ConcurrentLogging_LogsAllMessages()
        {
            // Arrange
            var messages = new[]
            {
                "Message 1",
                "Message 2",
                "Message 3"
            };

            // Act
            var tasks = new Task[messages.Length];
            for (int i = 0; i < messages.Length; i++)
            {
                var message = messages[i];
                tasks[i] = Task.Run(() => _app.Logger.Log(LogLevel.Info, message));
            }
            Task.WaitAll(tasks);

            // Assert
            Assert.IsTrue(File.Exists(_logFilePath));
            var logContent = File.ReadAllText(_logFilePath);
            foreach (var message in messages)
            {
                Assert.IsTrue(logContent.Contains(message));
            }
        }
    }
} 