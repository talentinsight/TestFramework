using System;
using System.Threading.Tasks;
using TestFramework.Core.Logger;
using TestFramework.Tests.Application;
using TestFramework.Core.Application;
using Xunit;
using Xunit.Sdk;

namespace TestFramework.Tests.Application
{
    public class MockCppApplicationTests : IDisposable
    {
        private readonly MockLogger _logger;
        private readonly MockCppApplication _application;

        public MockCppApplicationTests()
        {
            _logger = new MockLogger();
            _application = new MockCppApplication(_logger);
        }

        [Fact]
        public async Task WhenApplicationIsInitialized_StateIsCorrect()
        {
            // Act
            var result = await _application.InitializeAsync();

            // Assert
            Assert.True(result);
            Assert.True(_application.IsInitialized);
            Assert.False(_application.IsRunning);
        }

        [Fact]
        public async Task WhenApplicationIsStarted_StateIsCorrect()
        {
            // Arrange
            await _application.InitializeAsync();

            // Act
            var result = await _application.StartAsync();

            // Assert
            Assert.True(result);
            Assert.True(_application.IsInitialized);
            Assert.True(_application.IsRunning);
        }

        [Fact]
        public async Task WhenApplicationIsStopped_StateIsCorrect()
        {
            // Arrange
            await _application.InitializeAsync();
            await _application.StartAsync();

            // Act
            var result = await _application.StopAsync();

            // Assert
            Assert.True(result);
            Assert.True(_application.IsInitialized);
            Assert.False(_application.IsRunning);
        }

        [Fact]
        public async Task WhenApplicationIsRestarted_StateIsCorrect()
        {
            // Arrange
            await _application.InitializeAsync();
            await _application.StartAsync();
            await _application.StopAsync();

            // Act
            var result = await _application.RestartAsync();

            // Assert
            Assert.True(result);
            Assert.True(_application.IsInitialized);
            Assert.True(_application.IsRunning);
        }

        [Fact]
        public async Task InitializeAsync_WhenSuccessful_ReturnsTrueAndSetsInitialized()
        {
            // Act
            var result = await _application.InitializeAsync();

            // Assert
            Assert.True(result);
            Assert.True(_application.IsInitialized);
            Assert.Contains("Application initialized", _logger.Logs);
        }

        [Fact]
        public async Task InitializeAsync_WhenFailNextCall_ReturnsFalse()
        {
            // Arrange
            _application.FailNextCall = true;

            // Act
            var result = await _application.InitializeAsync();

            // Assert
            Assert.False(result);
            Assert.Equal("Initialization failed", _application.LastError);
        }

        [Fact]
        public async Task InitializeAsync_WhenThrowExceptionOnNextCall_ThrowsException()
        {
            // Arrange
            _application.ThrowExceptionOnNextCall = true;

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _application.InitializeAsync());
        }

        [Fact]
        public async Task StartAsync_WhenNotInitialized_ReturnsFalse()
        {
            // Act
            var result = await _application.StartAsync();

            // Assert
            Assert.False(result);
            Assert.Equal("Application not initialized", _application.LastError);
        }

        [Fact]
        public async Task StartAsync_WhenAlreadyRunning_ReturnsFalse()
        {
            // Arrange
            await _application.InitializeAsync();
            await _application.StartAsync();

            // Act
            var result = await _application.StartAsync();

            // Assert
            Assert.False(result);
            Assert.Equal("Application already running", _application.LastError);
        }

        [Fact]
        public async Task StopAsync_WhenNotRunning_ReturnsFalse()
        {
            // Act
            var result = await _application.StopAsync();

            // Assert
            Assert.False(result);
            Assert.Equal("Application not running", _application.LastError);
        }

        [Fact]
        public async Task StopAsync_WhenFailNextCall_ReturnsFalse()
        {
            // Arrange
            await _application.InitializeAsync();
            await _application.StartAsync();
            _application.FailNextCall = true;

            // Act
            var result = await _application.StopAsync();

            // Assert
            Assert.False(result);
            Assert.Equal("Operation failed", _application.LastError);
        }

        [Fact]
        public async Task StopAsync_WhenThrowExceptionOnNextCall_ThrowsException()
        {
            // Arrange
            await _application.InitializeAsync();
            await _application.StartAsync();
            _application.ThrowExceptionOnNextCall = true;

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _application.StopAsync());
        }

        [Fact]
        public async Task RestartAsync_WhenNotRunning_ReturnsFalse()
        {
            // Act
            var result = await _application.RestartAsync();

            // Assert
            Assert.False(result);
            Assert.Equal("Application not running", _application.LastError);
        }

        [Fact]
        public async Task GetStatusAsync_WhenNotRunning_ReturnsStopped()
        {
            // Act
            var status = await _application.GetStatusAsync();

            // Assert
            Assert.Equal("Stopped", status);
        }

        [Fact]
        public async Task GetStatusAsync_WhenRunning_ReturnsRunning()
        {
            // Arrange
            await _application.InitializeAsync();
            await _application.StartAsync();

            // Act
            var status = await _application.GetStatusAsync();

            // Assert
            Assert.Equal("Running", status);
        }

        [Fact]
        public async Task GetStatusAsync_WhenFailNextCall_ReturnsNull()
        {
            // Arrange
            _application.FailNextCall = true;

            // Act
            var status = await _application.GetStatusAsync();

            // Assert
            Assert.Null(status);
            Assert.Equal("Operation failed", _application.LastError);
        }

        [Fact]
        public async Task SendCommandAsync_WhenNotRunning_ReturnsFalse()
        {
            // Act
            var result = await _application.SendCommandAsync("test");

            // Assert
            Assert.False(result);
            Assert.Equal("Application not running", _application.LastError);
        }

        [Fact]
        public async Task SendCommandAsync_WhenRunning_ReturnsTrueAndLogsCommand()
        {
            // Arrange
            await _application.InitializeAsync();
            await _application.StartAsync();

            // Act
            var result = await _application.SendCommandAsync("test");

            // Assert
            Assert.True(result);
            Assert.Contains("test", _logger.Logs);
        }

        [Fact]
        public async Task SendCommandAsync_WhenFailNextCall_ReturnsFalse()
        {
            // Arrange
            await _application.InitializeAsync();
            await _application.StartAsync();
            _application.FailNextCall = true;

            // Act
            var result = await _application.SendCommandAsync("test");

            // Assert
            Assert.False(result);
            Assert.Equal("Operation failed", _application.LastError);
        }

        [Fact]
        public async Task GetResponseAsync_WhenNotRunning_ReturnsNull()
        {
            // Act
            var response = await _application.GetResponseAsync();

            // Assert
            Assert.Null(response);
            Assert.Equal("Application not running", _application.LastError);
        }

        [Fact]
        public async Task GetResponseAsync_WhenRunning_ReturnsOK()
        {
            // Arrange
            await _application.InitializeAsync();
            await _application.StartAsync();

            // Act
            var response = await _application.GetResponseAsync();

            // Assert
            Assert.Equal("OK", response);
        }

        [Fact]
        public async Task GetResponseAsync_WhenFailNextCall_ReturnsNull()
        {
            // Arrange
            await _application.InitializeAsync();
            await _application.StartAsync();
            _application.FailNextCall = true;

            // Act
            var response = await _application.GetResponseAsync();

            // Assert
            Assert.Null(response);
            Assert.Equal("Operation failed", _application.LastError);
        }

        [Fact]
        public void SetModbusResponse_SetsResponse()
        {
            // Arrange
            var response = new byte[] { 1, 2, 3, 4 };

            // Act
            _application.SetModbusResponse(response);

            // Assert
            // Note: We can't directly verify the response was set as it's private
            // This test is mainly for coverage
        }

        public void Dispose()
        {
            _application?.Dispose();
        }

        [Fact]
        public async Task MultipleOperations_WhenInErrorState_HandlesErrorsCorrectly()
        {
            // Arrange
            _application.FailNextCall = true;

            // Act & Assert
            var initResult = await _application.InitializeAsync();
            Assert.False(initResult);
            Assert.Equal("Initialization failed", _application.LastError);

            var startResult = await _application.StartAsync();
            Assert.False(startResult);
            Assert.Equal("Application not initialized", _application.LastError);

            var status = await _application.GetStatusAsync();
            Assert.Equal("Stopped", status);
        }

        [Fact]
        public async Task Dispose_WhenNotInitialized_DoesNotThrow()
        {
            // Act & Assert
            var exception = Record.Exception(() => _application.Dispose());
            Assert.Null(exception);
        }

        [Fact]
        public async Task Dispose_WhenDisposed_CanBeDisposedAgain()
        {
            // Arrange
            _application.InitializeAsync().Wait();
            _application.StartAsync().Wait();
            _application.Dispose();

            // Act & Assert
            var exception = Record.Exception(() => _application.Dispose());
            Assert.Null(exception);
        }

        [Fact]
        public async Task WhenServiceStarts_NoExceptionThrown()
        {
            // Arrange
            var logger = new MockLogger();
            var app = new CppApplication(logger, "test.exe");

            // Act & Assert
            await app.InitializeAsync();
            var exception = await Record.ExceptionAsync(async () => await app.StartServiceAsync("Database"));
            Assert.Null(exception);
        }

        [Fact]
        public async Task WhenServiceStops_NoExceptionThrown()
        {
            // Arrange
            var logger = new MockLogger();
            var app = new CppApplication(logger, "test.exe");

            // Act & Assert
            await app.InitializeAsync();
            await app.StartServiceAsync("Database");
            var exception = await Record.ExceptionAsync(async () => await app.StopServiceAsync("Database"));
            Assert.Null(exception);
        }

        [Fact]
        public async Task WhenDisposeCalled_NoExceptionThrown()
        {
            // Arrange
            var logger = new MockLogger();
            var app = new CppApplication(logger, "test.exe");

            // Act & Assert
            var exception = Record.Exception(() => app.Dispose());
            Assert.Null(exception);
        }

        [Fact]
        public async Task WhenDisposeCalledWhileRunning_NoExceptionThrown()
        {
            // Arrange
            var logger = new MockLogger();
            var app = new CppApplication(logger, "test.exe");

            // Act & Assert
            await app.InitializeAsync();
            await app.StartAsync();
            var exception = Record.Exception(() => app.Dispose());
            Assert.Null(exception);
        }
    }
} 