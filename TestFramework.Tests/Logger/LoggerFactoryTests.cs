using System;
<<<<<<< HEAD
=======
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
>>>>>>> df66c302549408ea17e5338bbce861a452d6d404
using TestFramework.Core.Logger;

namespace TestFramework.Tests.Logger
{
<<<<<<< HEAD
    public class LoggerFactoryTests
    {
        [Fact]
        public void CreateLogger_WithConsoleType_ReturnsConsoleLogger()
=======
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
>>>>>>> df66c302549408ea17e5338bbce861a452d6d404
        {
            // Act
            var logger = LoggerFactory.CreateLogger(LoggerType.Console);

            // Assert
<<<<<<< HEAD
            Assert.NotNull(logger);
            Assert.IsType<ConsoleLogger>(logger);
        }

        [Fact]
        public void CreateLogger_WithFileType_ReturnsFileLogger()
        {
            // Act
            var logger = LoggerFactory.CreateLogger(LoggerType.File, "test.log");

            // Assert
            Assert.NotNull(logger);
            Assert.IsType<FileLogger>(logger);
        }

        [Fact]
        public void CreateLogger_WithInvalidType_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => LoggerFactory.CreateLogger((LoggerType)999));
        }

        [Fact]
        public void CreateLogger_WithFileTypeAndNoPath_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => LoggerFactory.CreateLogger(LoggerType.File));
=======
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
>>>>>>> df66c302549408ea17e5338bbce861a452d6d404
        }
    }
} 