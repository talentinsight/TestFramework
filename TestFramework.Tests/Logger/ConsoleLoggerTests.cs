using System;
using System.IO;
using TestFramework.Core.Logger;

namespace TestFramework.Tests.Logger
{
    public class ConsoleLoggerTests : IDisposable
    {
        private readonly StringWriter _stringWriter;
        private readonly TextWriter _originalOutput;
        private readonly ConsoleLogger _logger;

        public ConsoleLoggerTests()
        {
            _stringWriter = new StringWriter();
            _originalOutput = Console.Out;
            Console.SetOut(_stringWriter);
            _logger = new ConsoleLogger();
        }

        public void Dispose()
        {
            _stringWriter.Dispose();
            Console.SetOut(_originalOutput);
        }

        [Theory]
        [InlineData(LogLevel.Debug, "Debug message")]
        [InlineData(LogLevel.Info, "Info message")]
        [InlineData(LogLevel.Warning, "Warning message")]
        [InlineData(LogLevel.Error, "Error message")]
        public void Log_WithDifferentLevels_WritesToConsole(LogLevel level, string message)
        {
            // Act
            _logger.Log(message, level);

            // Assert
            var output = _stringWriter.ToString();
            Assert.Contains($"[{level.ToString().ToUpper()}]", output);
            Assert.Contains(message, output);
        }

        [Fact]
        public void Log_WithoutLevel_UsesInfoLevel()
        {
            // Act
            _logger.Log("Test message");

            // Assert
            var output = _stringWriter.ToString();
            Assert.Contains("[INFO]", output);
            Assert.Contains("Test message", output);
        }

        [Fact]
        public void Log_WithLevelBelowCurrent_DoesNotLog()
        {
            // Arrange
            _logger.SetLogLevel(LogLevel.Warning);

            // Act
            _logger.Log("Test message", LogLevel.Info);

            // Assert
            var output = _stringWriter.ToString();
            Assert.DoesNotContain("[INFO]", output);
            Assert.DoesNotContain("Test message", output);
        }
    }
} 