using System;
using System.IO;
<<<<<<< HEAD
=======
using Microsoft.VisualStudio.TestTools.UnitTesting;
>>>>>>> df66c302549408ea17e5338bbce861a452d6d404
using TestFramework.Core.Logger;

namespace TestFramework.Tests.Logger
{
<<<<<<< HEAD
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
=======
    [TestClass]
    public class ConsoleLoggerTests
    {
        private StringWriter _consoleOutput = null!;
        private ILogger _logger = null!;
        private TextWriter _originalConsoleOutput = null!;

        [TestInitialize]
        public void Setup()
        {
            _originalConsoleOutput = Console.Out;
            _consoleOutput = new StringWriter();
            Console.SetOut(_consoleOutput);
            _logger = new ConsoleLogger();
        }

        [TestCleanup]
        public void Cleanup()
        {
            Console.SetOut(_originalConsoleOutput);
            _consoleOutput.Dispose();
        }

        [TestMethod]
        public void Log_InfoLevel_LogsCorrectly()
        {
            // Arrange
            var message = "Test info message";
>>>>>>> df66c302549408ea17e5338bbce861a452d6d404

        [Theory]
        [InlineData(LogLevel.Debug, "Debug message")]
        [InlineData(LogLevel.Info, "Info message")]
        [InlineData(LogLevel.Warning, "Warning message")]
        [InlineData(LogLevel.Error, "Error message")]
        public void Log_WithDifferentLevels_WritesToConsole(LogLevel level, string message)
        {
            // Act
<<<<<<< HEAD
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
=======
            _logger.Log(LogLevel.Info, message);

            // Assert
            var output = _consoleOutput.ToString();
            Assert.IsTrue(output.Contains(message));
            Assert.IsTrue(output.Contains("[Info]"));
        }

        [TestMethod]
        public void Log_WarningLevel_LogsCorrectly()
        {
            // Arrange
            var message = "Test warning message";

            // Act
            _logger.Log(LogLevel.Warning, message);

            // Assert
            var output = _consoleOutput.ToString();
            Assert.IsTrue(output.Contains(message));
            Assert.IsTrue(output.Contains("[Warning]"));
        }

        [TestMethod]
        public void Log_ErrorLevel_LogsCorrectly()
        {
            // Arrange
            var message = "Test error message";
            var exception = new Exception("Test exception");

            // Act
            _logger.Log(LogLevel.Error, message, exception);

            // Assert
            var output = _consoleOutput.ToString();
            Assert.IsTrue(output.Contains(message));
            Assert.IsTrue(output.Contains("[Error]"));
            Assert.IsTrue(output.Contains(exception.Message));
        }

        [TestMethod]
        public void Log_WithException_LogsExceptionDetails()
        {
            // Arrange
            var message = "Test error message";
            var exception = new InvalidOperationException("Test exception");

            // Act
            _logger.Log(LogLevel.Error, message, exception);

            // Assert
            var output = _consoleOutput.ToString();
            Assert.IsTrue(output.Contains(message));
            Assert.IsTrue(output.Contains("[Error]"));
            Assert.IsTrue(output.Contains(exception.Message));
            Assert.IsTrue(output.Contains(exception.GetType().Name));
>>>>>>> df66c302549408ea17e5338bbce861a452d6d404
        }
    }
} 