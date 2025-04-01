using NUnit.Framework;
using System.IO;
using TestFramework.Core.Logger;

namespace TestFramework.Tests.Logger
{
    [TestFixture]
    public class LoggerFactoryTests
    {
        private string _testLogFile;

        [SetUp]
        public void Setup()
        {
            _testLogFile = Path.Combine(Path.GetTempPath(), "test_log.txt");
            if (File.Exists(_testLogFile))
            {
                File.Delete(_testLogFile);
            }
        }

        [TearDown]
        public void TearDown()
        {
            if (File.Exists(_testLogFile))
            {
                File.Delete(_testLogFile);
            }
        }

        [Test]
        public void CreateLogger_WithConsoleType_ReturnsConsoleLogger()
        {
            // Arrange & Act
            var logger = LoggerFactory.CreateLogger(LoggerType.Console);

            // Assert
            Assert.That(logger, Is.TypeOf<ConsoleLogger>());
        }

        [Test]
        public void CreateLogger_WithFileType_ReturnsFileLogger()
        {
            // Arrange & Act
            var logger = LoggerFactory.CreateLogger(LoggerType.File, _testLogFile);

            // Assert
            Assert.That(logger, Is.TypeOf<FileLogger>());
        }

        [Test]
        public void CreateLogger_WithMockType_ReturnsMockLogger()
        {
            // Arrange & Act
            var logger = LoggerFactory.CreateLogger(LoggerType.Mock);

            // Assert
            Assert.That(logger, Is.TypeOf<MockLogger>());
        }

        [Test]
        public void CreateLogger_WithFileType_UsesDefaultPathWhenNull()
        {
            // Arrange & Act
            var logger = LoggerFactory.CreateLogger(LoggerType.File);

            // Assert
            Assert.That(logger, Is.TypeOf<FileLogger>());
            
            // Test the logger works with default path
            logger.Log("Test message");
            
            // No exception should be thrown
            Assert.Pass("FileLogger created with default path works correctly");
        }
    }
} 