using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestFramework.Core.Logger;

namespace TestFramework.Tests.Logger
{
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

            // Act
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
        }
    }
} 