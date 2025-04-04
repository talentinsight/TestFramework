using System;
using System.Threading.Tasks;
using TestFramework.Core.Application;
using TestFramework.Tests.Logger;
using Xunit;

namespace TestFramework.Tests.Application
{
    public class ServiceLifecycleTests : IDisposable
    {
        private readonly MockLogger _logger;
        private readonly CppApplication _application;

        public ServiceLifecycleTests()
        {
            _logger = new MockLogger();
            _application = new CppApplication(_logger, "test.exe");
        }

        public void Dispose()
        {
            _application.Dispose();
        }

        [Fact]
        public async Task WhenServiceStarts_StateIsCorrect()
        {
            // Arrange
            await _application.InitializeAsync();

            // Act
            var result = await _application.StartAsync();

            // Assert
            Assert.True(result);
            Assert.True(_application.IsRunning);
            Assert.False(_application.IsInErrorState);
        }

        [Fact]
        public async Task WhenServiceStops_StateIsCorrect()
        {
            // Arrange
            await _application.InitializeAsync();
            await _application.StartAsync();

            // Act
            var result = await _application.StopAsync();

            // Assert
            Assert.True(result);
            Assert.False(_application.IsRunning);
            Assert.False(_application.IsInErrorState);
        }

        [Fact]
        public async Task WhenServiceRestarts_StateIsCorrect()
        {
            // Arrange
            await _application.InitializeAsync();
            await _application.StartAsync();
            await _application.StopAsync();

            // Act
            var result = await _application.RestartAsync();

            // Assert
            Assert.True(result);
            Assert.True(_application.IsRunning);
            Assert.False(_application.IsInErrorState);
        }

        [Fact]
        public async Task WhenServiceFails_ErrorStateIsSet()
        {
            // Act
            var result = await _application.StartAsync();

            // Assert
            Assert.False(result);
            Assert.True(_application.IsInErrorState);
            Assert.Equal("Application not initialized", _application.LastError);
        }
    }
} 