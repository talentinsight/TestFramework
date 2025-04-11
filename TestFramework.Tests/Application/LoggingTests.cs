using System;
<<<<<<< HEAD
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
=======
using System.Threading.Tasks;
using TestFramework.Core.Application;
using TestFramework.Core.Logger;
using TestFramework.Tests.Logger;
using Xunit;

namespace TestFramework.Tests.Application
{
    public class LoggingTests : IDisposable
    {
        private readonly MockLogger _logger;
        private readonly CppApplication _application;

        public LoggingTests()
        {
            _logger = new MockLogger();
            _application = new CppApplication(_logger, "test.exe");
        }

        [Fact]
        public async Task WhenApplicationIsInitialized_LogsInitialization()
        {
            // Act
            await _application.InitializeAsync();

            // Assert
            Assert.Contains(_logger.Logs, log => log.Contains("[INFO]") && log.Contains("Application initialized"));
        }

        [Fact]
        public async Task WhenApplicationIsStarted_LogsStartup()
        {
            // Arrange
            await _application.InitializeAsync();

            // Act
            await _application.StartAsync();

            // Assert
            Assert.Contains(_logger.Logs, log => log.Contains("[INFO]") && log.Contains("Application started"));
        }

        [Fact]
        public async Task WhenApplicationIsStopped_LogsShutdown()
        {
            // Arrange
            await _application.InitializeAsync();
            await _application.StartAsync();

            // Act
            await _application.StopAsync();

            // Assert
            Assert.Contains(_logger.Logs, log => log.Contains("[INFO]") && log.Contains("Application stopped"));
        }

        [Fact]
        public async Task WhenApplicationIsRestarted_LogsRestart()
        {
            // Arrange
            await _application.InitializeAsync();
            await _application.StartAsync();
            await _application.StopAsync();

            // Act
            await _application.RestartAsync();

            // Assert
            Assert.Contains(_logger.Logs, log => log.Contains("[INFO]") && log.Contains("Application restarted"));
        }

        [Fact]
        public async Task WhenErrorOccurs_LogsError()
        {
            // Act
            await _application.StartAsync();

            // Assert
            Assert.Contains(_logger.Logs, log => log.Contains("[ERROR]") && log.Contains("Application not initialized"));
        }

        [Fact]
        public async Task WhenLogLevelIsSet_LogsAreFiltered()
        {
            // Arrange
            await _application.InitializeAsync();
            _application.SetLogLevel("WARNING");

            // Act
            await _application.StartAsync(); // This generates an INFO log
            await _application.StopAsync(); // This generates an INFO log
            _application.SetLogLevel("ERROR"); // This generates a WARNING log

            // Assert
            var logs = _logger.Logs;
            Assert.DoesNotContain(logs, log => log.Contains("[INFO]"));
            Assert.Contains(logs, log => log.Contains("[WARNING]"));
            Assert.Contains(logs, log => log.Contains("[ERROR]"));
        }

        [Fact]
        public void WhenInvalidLogLevelIsSet_ErrorIsReported()
        {
            // Act
            var result = _application.SetLogLevel("INVALID");

            // Assert
            Assert.False(result);
            Assert.Contains(_logger.Logs, log => log.Contains("[ERROR]") && log.Contains("Invalid log level"));
        }

        [Fact]
        public void WhenLogMessageIsSent_MessageIsLogged()
        {
            // Arrange
            const string testMessage = "Test log message";

            // Act
            _logger.Log(testMessage);

            // Assert
            Assert.Single(_logger.Logs);
            Assert.Contains(testMessage, _logger.Logs[0]);
        }

        [Fact]
        public void WhenMultipleLogsAreSent_AllMessagesAreLogged()
        {
            // Arrange
            const string message1 = "First message";
            const string message2 = "Second message";
            const string message3 = "Third message";

            // Act
            _logger.Log(message1);
            _logger.Log(message2);
            _logger.Log(message3);

            // Assert
            Assert.Equal(3, _logger.Logs.Count);
            Assert.Contains(message1, _logger.Logs[0]);
            Assert.Contains(message2, _logger.Logs[1]);
            Assert.Contains(message3, _logger.Logs[2]);
        }

        [Fact]
        public async Task WhenLogLevelIsChanged_NewLevelIsApplied()
        {
            // Arrange
            await _application.InitializeAsync();
            _application.SetLogLevel("INFO");
            _logger.Log("This should be logged", LogLevel.Info);
            _logger.Clear();

            // Act
            _application.SetLogLevel("ERROR");
            _logger.Log("This should not be logged", LogLevel.Info);
            _logger.Log("This should be logged", LogLevel.Error);

            // Assert
            Assert.Single(_logger.Logs);
            Assert.Contains("error", _logger.Logs[0]);
        }

        [Fact]
        public void Log_WithInfoLevel_AddsMessageToLogs()
        {
            var logger = new MockLogger();
            logger.Log("Test message", LogLevel.Info);
            Assert.Single(logger.Logs);
            Assert.Contains("Test message", logger.Logs[0]);
        }

        [Fact]
        public void Log_WithErrorLevel_AddsMessageToErrorMessages()
        {
            var logger = new MockLogger();
            logger.Log("Error message", LogLevel.Error);
            Assert.Single(logger.ErrorMessages);
            Assert.Equal("Error message", logger.ErrorMessages[0]);
        }

        [Fact]
        public void Clear_RemovesAllMessages()
        {
            var logger = new MockLogger();
            logger.Log("Test message", LogLevel.Info);
            logger.Log("Error message", LogLevel.Error);
            logger.Clear();
            Assert.Empty(logger.Logs);
            Assert.Empty(logger.ErrorMessages);
        }

        [Fact]
        public void SetLogLevel_WithValidLevel_ReturnsTrue()
        {
            // Act
            var result = _application.SetLogLevel("INFO");

            // Assert
            Assert.True(result);
            Assert.Equal("INFO", _application.GetLogLevel());
        }

        [Fact]
        public async Task GetRecentLogs_ReturnsCorrectNumberOfLogs()
        {
            // Arrange
            await _application.InitializeAsync();
            await _application.StartAsync();
            await _application.StopAsync();

            // Act
            var logs = _application.GetRecentLogs(2);

            // Assert
            Assert.Equal(2, logs.Count);
        }

        [Fact]
        public async Task ClearLogs_ClearsAllLogs()
        {
            // Arrange
            await _application.InitializeAsync();
            await _application.StartAsync();
            await _application.StopAsync();

            // Act
            var result = _application.ClearLogs();

            // Assert
            Assert.True(result);
            Assert.Single(_application.GetRecentLogs(10)); // Only the "Logs cleared" message
        }

        public void Dispose()
        {
            _application?.Dispose();
>>>>>>> d14476f86062bbdacd7e5bd7c4a9cb8565e91e68
        }
    }
} 