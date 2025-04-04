using System;
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
        }
    }
} 