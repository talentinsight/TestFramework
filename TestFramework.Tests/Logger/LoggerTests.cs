using System.Collections.Generic;
using Xunit;
using TestFramework.Core.Logger;
using System;
using System.IO;

namespace TestFramework.Tests.Logger
{
    public class LoggerTests
    {
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
        }
    }
}

/* 
 * Defines the LoggerTests class
 * Defines Log_WithDifferentLevels_LogsCorrectly function
 * Defines Log_WhenLevelIsBelowCurrent_DoesNotLog function
 * Defines Log_WithoutLevel_UsesInfoLevel function
 * Defines ClearLogs_RemovesAllLogs function
 */     
    