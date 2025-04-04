using System;
using System.Threading.Tasks;
using TestFramework.Core.Application;
using TestFramework.Tests.Logger;
using Xunit;

namespace TestFramework.Tests.Application
{
    public class ErrorHandlingTests : IDisposable
    {
        private readonly MockLogger _logger;
        private readonly CppApplication _application;

        public ErrorHandlingTests()
        {
            _logger = new MockLogger();
            _application = new CppApplication(_logger, "test.exe");
        }

        public void Dispose()
        {
            _application.Dispose();
        }

        [Fact]
        public async Task WhenErrorOccurs_ErrorStateIsSet()
        {
            // Act
            await _application.StartAsync();

            // Assert
            Assert.True(_application.IsInErrorState);
            Assert.Equal("Application not initialized", _application.LastError);
        }

        [Fact]
        public async Task WhenErrorClears_ErrorStateIsReset()
        {
            // Arrange
            await _application.StartAsync();
            Assert.True(_application.IsInErrorState);

            // Act
            await _application.InitializeAsync();

            // Assert
            Assert.False(_application.IsInErrorState);
            Assert.Empty(_application.LastError);
        }

        [Fact]
        public async Task WhenMultipleErrorsOccur_LastErrorIsUpdated()
        {
            // Arrange
            await _application.StartAsync();
            Assert.Equal("Application not initialized", _application.LastError);

            // Act
            await _application.StopAsync();

            // Assert
            Assert.Equal("Application not running", _application.LastError);
        }

        [Fact]
        public async Task WhenErrorOccurs_LogsError()
        {
            // Act
            await _application.StartAsync();

            // Assert
            Assert.Contains(_logger.Logs, log => log.Contains("[ERROR]") && log.Contains("Application not initialized"));
        }
    }
} 