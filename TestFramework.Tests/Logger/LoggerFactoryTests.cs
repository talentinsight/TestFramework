using System;
using TestFramework.Core.Logger;

namespace TestFramework.Tests.Logger
{
    public class LoggerFactoryTests
    {
        [Fact]
        public void CreateLogger_WithConsoleType_ReturnsConsoleLogger()
        {
            // Act
            var logger = LoggerFactory.CreateLogger(LoggerType.Console);

            // Assert
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
        }
    }
} 