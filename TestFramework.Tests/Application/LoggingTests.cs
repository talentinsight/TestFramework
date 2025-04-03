using System;
using System.Threading.Tasks;
using NUnit.Framework;
using TestFramework.Core.Application;
using TestFramework.Core.Logger;

namespace TestFramework.Tests.Application
{
    [TestFixture]
    public class LoggingTests
    {
        private MockCppApplication _application;
        private MockLogger _logger;

        [SetUp]
        public void Setup()
        {
            _logger = new MockLogger();
            _application = new MockCppApplication();
        }

        [Test]
        public async Task WhenLogLevelIsSet_LogsAreFiltered()
        {
            // Arrange
            _application.SetLogLevel("WARN");

            // Act
            await _application.StartAsync();
            _logger.Log("This is a debug message", LogLevel.Debug);
            _logger.Log("This is a warning message", LogLevel.Warning);
            _logger.Log("This is an error message", LogLevel.Error);

            // Assert
            Assert.That(_logger.LogMessages, Has.Count.EqualTo(2));
            Assert.That(_logger.LogMessages[0], Contains.Substring("warning"));
            Assert.That(_logger.LogMessages[1], Contains.Substring("error"));
        }

        [Test]
        public async Task WhenLogMessageIsSent_MessageIsLogged()
        {
            // Arrange
            const string testMessage = "Test log message";

            // Act
            _logger.Log(testMessage);

            // Assert
            Assert.That(_logger.LogMessages, Has.Count.EqualTo(1));
            Assert.That(_logger.LogMessages[0], Contains.Substring(testMessage));
        }

        [Test]
        public async Task WhenMultipleLogsAreSent_AllMessagesAreLogged()
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
            Assert.That(_logger.LogMessages, Has.Count.EqualTo(3));
            Assert.That(_logger.LogMessages[0], Contains.Substring(message1));
            Assert.That(_logger.LogMessages[1], Contains.Substring(message2));
            Assert.That(_logger.LogMessages[2], Contains.Substring(message3));
        }

        [Test]
        public async Task WhenLogLevelIsChanged_NewLevelIsApplied()
        {
            // Arrange
            _application.SetLogLevel("INFO");
            _logger.Log("This should be logged", LogLevel.Info);
            _logger.Clear();

            // Act
            _application.SetLogLevel("ERROR");
            _logger.Log("This should not be logged", LogLevel.Info);
            _logger.Log("This should be logged", LogLevel.Error);

            // Assert
            Assert.That(_logger.LogMessages, Has.Count.EqualTo(1));
            Assert.That(_logger.LogMessages[0], Contains.Substring("error"));
        }
    }
} 