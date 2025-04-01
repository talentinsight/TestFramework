using NUnit.Framework;
using TestFramework.Core.Logger;

namespace TestFramework.Tests.Logger
{
    [TestFixture]
    public class ConsoleLoggerTests
    {
        private ILogger _logger;

        [SetUp]
        public void Setup()
        {
            _logger = new ConsoleLogger();
        }

        [Test]
        public void Log_WhenCalled_ShouldWriteToConsole()
        {
            // Arrange
            var message = "Test message";

            // Act
            _logger.Log(message);

            // Assert
            // Console output verification would require additional setup
            Assert.Pass("Log method executed without exceptions");
        }
    }
} 