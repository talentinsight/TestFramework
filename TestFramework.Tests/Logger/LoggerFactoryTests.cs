using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestFramework.Core.Logger;

namespace TestFramework.Tests.Logger
{
    [TestClass]
    public class LoggerFactoryTests
    {
        private string _logFilePath;

        [TestInitialize]
        public void Setup()
        {
            _logFilePath = Path.Combine(Path.GetTempPath(), $"test_log_{Guid.NewGuid()}.txt");
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
        public void CreateLogger_ConsoleType_ReturnsConsoleLogger()
        {
            // Act
            var logger = LoggerFactory.CreateLogger(LoggerType.Console);

            // Assert
            Assert.IsInstanceOfType(logger, typeof(ConsoleLogger));
        }

        [TestMethod]
        public void CreateLogger_FileType_ReturnsFileLogger()
        {
            // Act
            var logger = LoggerFactory.CreateLogger(LoggerType.File, _logFilePath);

            // Assert
            Assert.IsInstanceOfType(logger, typeof(FileLogger));
        }

        [TestMethod]
        public void CreateLogger_MockType_ReturnsMockLogger()
        {
            // Act
            var logger = LoggerFactory.CreateLogger(LoggerType.Mock);

            // Assert
            Assert.IsInstanceOfType(logger, typeof(MockLogger));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateLogger_InvalidType_ThrowsArgumentException()
        {
            // Act
            LoggerFactory.CreateLogger((LoggerType)999);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreateLogger_FileTypeWithoutPath_ThrowsArgumentNullException()
        {
            // Act
            LoggerFactory.CreateLogger(LoggerType.File);
        }
    }
} 